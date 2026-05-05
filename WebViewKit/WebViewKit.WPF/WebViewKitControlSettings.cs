using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace WebViewKit.WPF
{
    /// <summary>
    /// WPF WebViewKitControl 전용 설정 그룹입니다.
    /// </summary>
    public class WebViewKitControlSettings : DependencyObject
    {
        // 설정 변경 시 컨트롤에 알리기 위한 이벤트
        internal event EventHandler? PropertyChanged;

        private static void OnSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebViewKitControlSettings s) s.PropertyChanged?.Invoke(s, EventArgs.Empty);
        }

        /// <summary>
        /// <inheritdoc cref="WebViewDescriptions.IsCrawlingMode"/>
        /// </summary>
        public bool IsCrawlingMode
        {
            get => (bool)GetValue(IsCrawlingModeProperty);
            set => SetValue(IsCrawlingModeProperty, value);
        }
        public static readonly DependencyProperty IsCrawlingModeProperty =
            DependencyProperty.Register("IsCrawlingMode", typeof(bool), typeof(WebViewKitControlSettings), new PropertyMetadata(false, OnSettingChanged));

        /// <summary>
        /// <inheritdoc cref="WebViewDescriptions.FirstUri"/>
        /// </summary>
        public string FirstUri
        {
            get => (string)GetValue(FirstUriProperty);
            set => SetValue(FirstUriProperty, value);
        }
        public static readonly DependencyProperty FirstUriProperty =
            DependencyProperty.Register("FirstUri", typeof(string), typeof(WebViewKitControlSettings), new PropertyMetadata("about:blank", OnSettingChanged));

        // --- 브라우저 기본 설정들 ---
        /// <summary>
        /// <inheritdoc cref="WebViewDescriptions.AreDefaultScriptDialogsEnabled"/>
        /// </summary>
        public bool AreDefaultScriptDialogsEnabled
        {
            get => (bool)GetValue(AreDefaultScriptDialogsEnabledProperty);
            set => SetValue(AreDefaultScriptDialogsEnabledProperty, value);
        }
        public static readonly DependencyProperty AreDefaultScriptDialogsEnabledProperty =
            DependencyProperty.Register("AreDefaultScriptDialogsEnabled", typeof(bool), typeof(WebViewKitControlSettings), new PropertyMetadata(true, OnSettingChanged));

        /// <summary>
        /// <inheritdoc cref="WebViewDescriptions.IsStatusBarEnabled"/>
        /// </summary>
        public bool IsStatusBarEnabled
        {
            get => (bool)GetValue(IsStatusBarEnabledProperty);
            set => SetValue(IsStatusBarEnabledProperty, value);
        }
        public static readonly DependencyProperty IsStatusBarEnabledProperty =
            DependencyProperty.Register("IsStatusBarEnabled", typeof(bool), typeof(WebViewKitControlSettings), new PropertyMetadata(false, OnSettingChanged));

        /// <summary>
        /// <inheritdoc cref="WebViewDescriptions.AreDevToolsEnabled"/>
        /// </summary>
        public bool AreDevToolsEnabled
        {
            get => (bool)GetValue(AreDevToolsEnabledProperty);
            set => SetValue(AreDevToolsEnabledProperty, value);
        }
        public static readonly DependencyProperty AreDevToolsEnabledProperty =
            DependencyProperty.Register("AreDevToolsEnabled", typeof(bool), typeof(WebViewKitControlSettings), new PropertyMetadata(true, OnSettingChanged));

        /// <summary>
        /// <inheritdoc cref="WebViewDescriptions.IsWebMessageEnabled"/>
        /// </summary>
        public bool IsWebMessageEnabled
        {
            get => (bool)GetValue(IsWebMessageEnabledProperty);
            set => SetValue(IsWebMessageEnabledProperty, value);
        }
        public static readonly DependencyProperty IsWebMessageEnabledProperty =
            DependencyProperty.Register("IsWebMessageEnabled", typeof(bool), typeof(WebViewKitControlSettings), new PropertyMetadata(true, OnSettingChanged));

        public override string ToString() => IsCrawlingMode ? "Crawling Mode" : "Normal Mode";
    }
}
