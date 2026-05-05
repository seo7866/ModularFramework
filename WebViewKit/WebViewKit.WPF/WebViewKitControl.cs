using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace WebViewKit.WPF
{
    public class WebViewKitControl : WebView2
    {
        public WebViewKitControl()
        {
            this.WebViewKitSettings = new WebViewKitControlSettings();
            this.WebViewKitSettings.PropertyChanged += OnSettingsChanged;

            // 컨트롤이 로드될 때 자동 초기화
            base.Loaded += WebViewKitControl_Loaded;
            base.Unloaded += WebViewKitControl_Unloaded;
        }

        /// <summary>
        /// WPF 의존성 속성으로 등록된 설정 그룹
        /// </summary>
        public WebViewKitControlSettings WebViewKitSettings
        {
            get { return (WebViewKitControlSettings)GetValue(WebViewKitSettingsProperty); }
            set { SetValue(WebViewKitSettingsProperty, value); }
        }
        public static readonly DependencyProperty WebViewKitSettingsProperty =
            DependencyProperty.Register("SharedSettings", typeof(WebViewKitControlSettings), typeof(WebViewKitControl), new PropertyMetadata(null));

        private async void WebViewKitControl_Loaded(object sender, RoutedEventArgs e)
        {
            base.Loaded -= WebViewKitControl_Loaded;
            // 디자이너 모드 체크 (WPF 방식)
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            try
            {
                // 1. 엔진 초기화
                await this.EnsureCoreWebView2Async();

                // 2. 공통 확장 메서드 등록 (WebViewKit 프로젝트 참조 필요)
                await this.CoreWebView2.InitializeAsync();
                await ApplySettingsInternalAsync();
                await this.CoreWebView2.NavigateWithAwaitAsync(WebViewKitSettings.FirstUri);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WebViewKitControl.WPF] 초기화 실패: {ex.Message}");
            }
        }

        private void WebViewKitControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // 이벤트 해제 (트렌드 반영 및 안전성 확보)
            base.Loaded -= WebViewKitControl_Loaded;
            base.Unloaded -= WebViewKitControl_Unloaded;
            WebViewKitSettings?.PropertyChanged -= OnSettingsChanged;

            // 엔진 자원 정리
            base.CoreWebView2?.DisposeWebView();
        }

        private void OnSettingsChanged(object sender, EventArgs e) => ApplySettings();

        public void ApplySettings()
        {
            if (this.CoreWebView2 == null || DesignerProperties.GetIsInDesignMode(this)) return;

            // Fire and Forget 패턴으로 비동기 업데이트 실행
            _ = ApplySettingsInternalAsync();
        }

        private async Task ApplySettingsInternalAsync()
        {
            try
            {
                await this.CoreWebView2.UpdateSetting(
                    WebViewKitSettings.IsCrawlingMode,
                    WebViewKitSettings.AreDefaultScriptDialogsEnabled,
                    WebViewKitSettings.IsStatusBarEnabled,
                    WebViewKitSettings.AreDevToolsEnabled,
                    WebViewKitSettings.IsWebMessageEnabled);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WebViewKitControl.WPF] 설정 업데이트 실패: {ex.Message}");
            }
        }
    }
}
