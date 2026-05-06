using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WebViewKit.WinForms
{
    public class WebViewKitControl : WebView2
    {
        private readonly WebViewKitControlSettings _settings = new();

        /// <summary>
        /// 웹뷰의 통합 설정 그룹입니다. (속성창에서 계층형으로 표시됨)
        /// </summary>
        [Category("WebViewKit"), Description("웹뷰 동작 설정을 통합 관리합니다.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public WebViewKitControlSettings WebViewKitSettings => _settings;

        public WebViewKitControl()
        {
            // 프로퍼티가 변경될 때마다 브라우저 엔진에 즉시 반영
            _settings.PropertyChanged += (s, e) => ApplySettings();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode) _ = InitializeInternalAsync();
        }

        private async Task InitializeInternalAsync()
        {
            try
            {
                // 1. 엔진 초기화 대기
                await this.EnsureCoreWebView2Async();

                // 2. 공통 확장 메서드에 등록 (락 생성 등)
                this.CoreWebView2.Initialize();
                ApplySettingsInternal();
                await this.CoreWebView2.NavigateWithAwaitAsync(WebViewKitSettings.FirstUri);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WebViewKitControl] 초기화 실패: {ex.Message}");
            }
        }

        public async Task<NavigationResult> NavigateWithAwaitAsync(string url, int timeout = 60000)
            => await this.CoreWebView2.NavigateWithAwaitAsync(url, timeout);

        public async Task<bool> DownloadFileAsync(string fullPath)
            => await this.CoreWebView2.DownloadFileAsync(fullPath);

        public async Task<string> GetCurrentHtmlAsync()
            => await this.CoreWebView2.GetCurrentHtmlAsync();

        public void ApplySettings()
        {
            // 디자인 모드이거나 엔진이 초기화 전이면 무시
            if (this.DesignMode || this.CoreWebView2 == null) 
                return;

            ApplySettingsInternal();
        }

        private void ApplySettingsInternal()
        {
            try
            {
                // 락(SemaphoreSlim)이 포함된 UpdateSetting을 안전하게 대기
                this.CoreWebView2.UpdateSetting(
                    this.WebViewKitSettings.IsCrawlingMode,
                    this.WebViewKitSettings.AreDefaultScriptDialogsEnabled,
                    this.WebViewKitSettings.IsStatusBarEnabled,
                    this.WebViewKitSettings.AreDevToolsEnabled,
                    this.WebViewKitSettings.IsWebMessageEnabled,
                    this.WebViewKitSettings.IsPopupBlocked);
            }
            catch (Exception ex)
            {
                // 비동기 실행 중 발생한 예외 로깅
                System.Diagnostics.Debug.WriteLine($"[WebViewKitControl] 설정 업데이트 실패: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.CoreWebView2 != null)
            {
                this.CoreWebView2.DisposeWebView();
            }
            base.Dispose(disposing);
        }
    }
}
