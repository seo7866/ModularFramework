using Microsoft.Web.WebView2.Core;
using System.Collections.Concurrent;
using System.Text.Json;

namespace WebViewKit
{
    public static class WebViewCoreExtensions
    {
        private static readonly ConcurrentDictionary<CoreWebView2, WebViewState> States = new();

        public static void Initialize(this CoreWebView2 coreWebView)
        {
            ArgumentNullException.ThrowIfNull(coreWebView);

            States.TryAdd(coreWebView, new WebViewState());

            coreWebView.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            coreWebView.NewWindowRequested += CoreWebView_NewWindowRequested;
            coreWebView.WebResourceRequested += CoreWebView_WebResourceRequested;
        }

        public static async Task<NavigationResult> NavigateWithAwaitAsync(this CoreWebView2 coreWebView, string url, int timeout = 60000)
        {
            var tcs = new TaskCompletionSource<NavigationResult>();
            void Handler(object sender, CoreWebView2NavigationCompletedEventArgs e)
            {
                coreWebView.NavigationCompleted -= Handler;

                tcs.TrySetResult(new NavigationResult
                {
                    IsSuccess = e.IsSuccess,
                    EventArgs = e
                });
            }

            coreWebView.NavigationCompleted += Handler;
            coreWebView.Navigate(url);
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
            if (completed != tcs.Task)
            {
                coreWebView.NavigationCompleted -= Handler;
                return new NavigationResult
                {
                    IsTimeout = true,
                    IsSuccess = false
                };
            }

            return await tcs.Task;
        }

        public static async Task<bool> DownloadFileAsync(this CoreWebView2 coreWebView, string fullPath)
        {
            var tcs = new TaskCompletionSource<bool>();

            void OnDownload(object sender, CoreWebView2DownloadStartingEventArgs e)
            {
                coreWebView.DownloadStarting -= OnDownload;

                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                e.ResultFilePath = fullPath;
                e.Handled = true;

                void StateChanged(object s, object ev)
                {
                    var op = e.DownloadOperation;
                    if (op.State == CoreWebView2DownloadState.Completed)
                        tcs.TrySetResult(true);

                    if (op.State == CoreWebView2DownloadState.Interrupted)
                        tcs.TrySetException(new Exception(op.InterruptReason.ToString()));
                }

                e.DownloadOperation.StateChanged += StateChanged;
            }

            coreWebView.DownloadStarting += OnDownload;

            return await tcs.Task;
        }

        public static async Task<string> GetCurrentHtmlAsync(this CoreWebView2 coreWebView)
        {
            var raw = await coreWebView.ExecuteScriptAsync("document.documentElement.outerHTML");

            if (string.IsNullOrWhiteSpace(raw) || raw == "null")
                return string.Empty;

            try
            {
                return JsonSerializer.Deserialize<string>(raw) ?? string.Empty;
            }
            catch
            {
                return raw.Trim('"')
                          .Replace("\\u003C", "<")
                          .Replace("\\u003E", ">")
                          .Replace("\\\"", "\"")
                          .Replace("\\\\", "\\");
            }
        }

        public static void UpdateSetting(this CoreWebView2 coreWebView, bool isCrawlingMode,
                bool areDefaultScriptDialogsEnabled, bool isStatusBarEnabled, bool areDevToolsEnabled, bool isWebMessageEnabled,
                bool isPopupBlocked)
        {
            coreWebView.Settings.AreDefaultScriptDialogsEnabled = areDefaultScriptDialogsEnabled;
            coreWebView.Settings.IsStatusBarEnabled = isStatusBarEnabled;
            coreWebView.Settings.AreDevToolsEnabled = areDevToolsEnabled;
            coreWebView.Settings.IsWebMessageEnabled = isWebMessageEnabled;
            coreWebView.UpdateWebViewInfo(isPopupBlocked, isCrawlingMode);
        }

        public static void UpdateWebViewInfo(this CoreWebView2 coreWebView, bool isPopupBlocked, bool isCrawlingMode)
        {
            var state = coreWebView.GetState();
            state.IsCrawlingMode = isCrawlingMode;
            state.IsPopupBlocked = isPopupBlocked;
        }

        public static void DisposeWebView(this CoreWebView2 coreWebView)
        {
            coreWebView.WebResourceRequested -= CoreWebView_WebResourceRequested;
            coreWebView.NewWindowRequested -= CoreWebView_NewWindowRequested;
            States.TryRemove(coreWebView, out _);
        }

        #region Event

        private static void CoreWebView_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            var info = (sender as CoreWebView2).GetState();
            e.Handled = info.IsPopupBlocked;
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

            if (sender is not CoreWebView2 coreWebView || !(coreWebView.GetState()).IsCrawlingMode)
                return;
            // 불필요한 리소스(이미지/CSS 등) 로딩 차단 로직
            if (IsRedundantResource(e.ResourceContext, e.Request.Uri))
            {
                e.Response = coreWebView.Environment.CreateWebResourceResponse(null, 403, "Blocked", null);
            }
        }

        #endregion

        private static WebViewState GetState(this CoreWebView2 coreWebView)
        {
            if (!States.TryGetValue(coreWebView, out var value) || value == null)
            {
                throw new InvalidOperationException(
                    "WebViewCoreExtensions: InitializeAsync를 먼저 호출하여 웹뷰를 초기화해야 합니다.");
            }
            return value;
        }

        private class WebViewState
        {
            public bool IsCrawlingMode { get; set; }
            public bool IsPopupBlocked { get; set; }

            public bool IsNavigating { get; set; }
        }
    }
}
