using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WebViewKit
{
    public static class WebViewCoreExtensions
    {
        private static readonly ConcurrentDictionary<CoreWebView2, SemaphoreSlim> WebView2Locks = new();
        private static readonly ConcurrentDictionary<CoreWebView2, bool> WebView2IsCrawlingMode = new();

        public static async Task InitializeAsync(this CoreWebView2 coreWebView)
        {
            WebView2Locks.TryAdd(coreWebView, new SemaphoreSlim(1, 1));
            WebView2IsCrawlingMode.TryAdd(coreWebView, false);

            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        }

        public static async Task UpdateSetting(this CoreWebView2 coreWebView, bool isCrawlingMode,
            bool areDefaultScriptDialogsEnabled, bool isStatusBarEnabled, bool areDevToolsEnabled, bool isWebMessageEnabled)
        {
            coreWebView.Settings.AreDefaultScriptDialogsEnabled = areDefaultScriptDialogsEnabled;
            coreWebView.Settings.IsStatusBarEnabled = isStatusBarEnabled;
            coreWebView.Settings.AreDevToolsEnabled = areDevToolsEnabled;
            coreWebView.Settings.IsWebMessageEnabled = isWebMessageEnabled;
            await coreWebView.UpdateIsCrawlingMode(isCrawlingMode);
        }

        public static async Task UpdateIsCrawlingMode(this CoreWebView2 coreWebView, bool enable)
        {
            var semaphore = coreWebView.GetSemaphoreSlim();
            await semaphore.WaitAsync();

            try
            {
                if (coreWebView.GetIsCrawlingMode() == enable)
                    return;

                //실제 이벤트 등록/해제 처리
                if (enable)
                {
                    coreWebView.WebResourceRequested += CoreWebView_WebResourceRequested;
                }
                else
                {
                    coreWebView.WebResourceRequested -= CoreWebView_WebResourceRequested;
                }
                //상태 저장소 업데이트
                WebView2IsCrawlingMode[coreWebView] = enable;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static void DisposeWebView(this CoreWebView2 coreWebView)
        {
            // 등록된 자물쇠와 상태값 제거
            WebView2Locks.TryRemove(coreWebView, out _);
            WebView2IsCrawlingMode.TryRemove(coreWebView, out _);
        }

        private static void CoreWebView_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            static bool IsRedundantResource(CoreWebView2WebResourceContext context, string uri)
            {
                return context is CoreWebView2WebResourceContext.Image
                               or CoreWebView2WebResourceContext.Stylesheet
                               or CoreWebView2WebResourceContext.Font
                               or CoreWebView2WebResourceContext.Media
                       || uri.Contains("google-analytics");
            }

            if (sender is not CoreWebView2 coreWebView) return;
            // 불필요한 리소스(이미지/CSS 등) 로딩 차단 로직
            if (IsRedundantResource(e.ResourceContext, e.Request.Uri))
            {
                e.Response = coreWebView.Environment.CreateWebResourceResponse(null, 403, "Blocked", null);
            }
        }

        public static async Task<CoreWebView2NavigationCompletedEventArgs> NavigateWithAwaitAsync(this CoreWebView2 coreWebView,
            string url,
            int timeoutMilliseconds = 60000)
        {
            var semaphore = coreWebView.GetSemaphoreSlim();
            await semaphore.WaitAsync();
            var tcs = new TaskCompletionSource<CoreWebView2NavigationCompletedEventArgs>();
            bool isRemoved = false; // 이벤트가 제거되었는지 추적하는 플래그
            // 일회성 이벤트 핸들러 정의
            void Handler(object sender, CoreWebView2NavigationCompletedEventArgs e)
            {
                if (!isRemoved)
                {
                    coreWebView.NavigationCompleted -= Handler;
                    isRemoved = true;
                }
                tcs.TrySetResult(e);
            }

            try
            {
                coreWebView.NavigationCompleted += Handler;
                coreWebView.Navigate(url);
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMilliseconds));
                if (completedTask == tcs.Task)
                {
                    // 성공적으로 탐색 완료 (이벤트 결과 반환)
                    return await tcs.Task;
                }
                else
                {
                    // 타임아웃 발생
                    throw new TimeoutException($"WebViewKit: '{url}' 주소로의 탐색 시간이 초과되었습니다. ({timeoutMilliseconds}ms)");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"WebViewKit: Navigate 호출 중 오류가 발생했습니다. (URL: {url})", ex);
            }
            finally
            {
                if (!isRemoved)
                {
                    coreWebView.NavigationCompleted -= Handler;
                    isRemoved = true;
                }
                semaphore.Release();
            }
        }

        public static async Task DownloadFile(this CoreWebView2 coreWebView, string fullPath,
            IProgress<double> progress = null, // 진행률 보고용 (0~100)
            int timeoutMs = 300000)
        {
            static string GenerateDownloadScript(string url, string fileName) => $@"
    (function() {{
        const a = document.createElement('a');
        a.href = '{url}';
        a.download = '{fileName}';
        document.body.appendChild(a);
        a.click();
        a.remove();
    }})();";

            var tcs = new TaskCompletionSource<bool>();
            bool isRemoved = false;

            void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
            {
                if (!isRemoved) { coreWebView.DownloadStarting -= CoreWebView2_DownloadStarting; isRemoved = true; }

                // 디렉토리 생성 로직
                string dir = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

                e.ResultFilePath = fullPath;
                e.Handled = true;
                // 중요: 내부 상태 변경 이벤트 정의
                void stateHandler(object s, object ev)
                {
                    // 1. 진행률 계산
                    // 에러 메시지로 보아 이미 ulong 형식이므로 바로 가져옵니다.
                    var total = e.DownloadOperation.TotalBytesToReceive;
                    var received = e.DownloadOperation.BytesReceived;

                    if (total > 0)
                    {
                        // ulong을 double로 캐스팅하여 계산 (가장 안전한 방법)
                        double p = (double)received / (double)total * 100;
                        progress?.Report(Math.Round(p, 1));
                    }

                    // 완료 혹은 중단된 '최종 상태'인지 확인
                    bool isFinished = e.DownloadOperation.State == CoreWebView2DownloadState.Completed ||
                                      e.DownloadOperation.State == CoreWebView2DownloadState.Interrupted;

                    if (isFinished)
                    {
                        try
                        {
                            if (e.DownloadOperation.State == CoreWebView2DownloadState.Completed)
                                tcs.TrySetResult(true);
                            else
                                tcs.TrySetException(new Exception($"다운로드 중단: {e.DownloadOperation.InterruptReason}"));
                        }
                        finally
                        {
                            e.DownloadOperation.StateChanged -= stateHandler; // 최종 상태일 때만 확실히 해제
                        }
                    }

                }

                e.DownloadOperation.StateChanged += stateHandler;
            }

            coreWebView.DownloadStarting += CoreWebView2_DownloadStarting;

            try
            {
                // 스크립트 실행 전 약간의 대기 (안정성)
                await coreWebView.ExecuteScriptAsync(GenerateDownloadScript(coreWebView.Source, Path.GetFileName(fullPath)));

                // 타임아웃 적용 (다운로드는 오래 걸릴 수 있으므로 넉넉히)
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
                if (completedTask != tcs.Task) throw new TimeoutException("다운로드 시간 초과");

                await tcs.Task;
            }
            finally
            {
                if (!isRemoved) { coreWebView.DownloadStarting -= CoreWebView2_DownloadStarting; isRemoved = true; }
            }
        }

        public static async Task<string> GetHtml(this CoreWebView2 coreWebView)
        {
            // 1. 스크립트 실행 (결과는 "<html>...</html>" 형태의 JSON 문자열)
            string rawJson = await coreWebView.ExecuteScriptAsync("document.documentElement.outerHTML");

            if (string.IsNullOrWhiteSpace(rawJson) || rawJson == "null") return string.Empty;

            try
            {
                // 2. .NET 내장 JSON 변환기 사용 (가장 안전하고 빠름)
                // rawJson 자체가 "\"<html>...\"" 형태이므로 한 번 역직렬화하면 끝납니다.
                return JsonSerializer.Deserialize<string>(rawJson) ?? string.Empty;
            }
            catch
            {
                // 만약의 경우를 대비한 폴백 로직
                return rawJson.Trim('"')
                              .Replace("\\u003C", "<")
                              .Replace("\\u003E", ">")
                              .Replace("\\\"", "\"")
                              .Replace("\\\\", "\\");
            }
        }

        private static SemaphoreSlim GetSemaphoreSlim(this CoreWebView2 coreWebView)
        {
            if (!WebView2Locks.TryGetValue(coreWebView, out SemaphoreSlim semaphore) || semaphore == null)
                throw new InvalidOperationException(
                    "WebViewCoreExtensions: InitializeAsync를 먼저 호출하여 웹뷰를 초기화해야 합니다.");
            return semaphore;
        }

        private static bool GetIsCrawlingMode(this CoreWebView2 coreWebView)
        {
            if (!WebView2IsCrawlingMode.TryGetValue(coreWebView, out bool isCrawlingMode))
                throw new InvalidOperationException(
                    "WebViewCoreExtensions: InitializeAsync를 먼저 호출하여 웹뷰를 초기화해야 합니다.");
            return isCrawlingMode;
        }
    }
}
