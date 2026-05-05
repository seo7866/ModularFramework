using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WebViewKit.WinForms
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WebViewKitControlSettings
    {
        private bool _isCrawlingMode = false;

        /// <summary>
        /// 설정 변경 시 컨트롤에 즉시 반영하기 위한 이벤트입니다.
        /// </summary>
        internal event EventHandler? PropertyChanged;

        [Category("Custom"), DefaultValue(false)]
        [Description(WebViewDescriptions.IsCrawlingMode)]
        public bool IsCrawlingMode
        {
            get => _isCrawlingMode;
            set { _isCrawlingMode = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        [Category("Custom"), DefaultValue("about:blank")]
        [Description(WebViewDescriptions.FirstUri)]
        public string FirstUri { get; set; } = "about:blank";

        [Category("Browser"), DefaultValue(true)]
        [Description(WebViewDescriptions.AreDefaultScriptDialogsEnabled)]
        public bool AreDefaultScriptDialogsEnabled { get; set; } = true;

        [Category("Browser"), DefaultValue(false)]
        [Description(WebViewDescriptions.IsStatusBarEnabled)]
        public bool IsStatusBarEnabled { get; set; } = false;

        [Category("Browser"), DefaultValue(true)]
        [Description(WebViewDescriptions.AreDevToolsEnabled)]
        public bool AreDevToolsEnabled { get; set; } = true;

        [Category("Browser"), DefaultValue(true)]
        [Description(WebViewDescriptions.IsWebMessageEnabled)]
        public bool IsWebMessageEnabled { get; set; } = true;

        /// <summary>
        /// 속성창에 표시될 요약 텍스트입니다.
        /// </summary>
        public override string ToString() => IsCrawlingMode ? "Crawling Mode (Active)" : "Normal Mode";
    }
}
