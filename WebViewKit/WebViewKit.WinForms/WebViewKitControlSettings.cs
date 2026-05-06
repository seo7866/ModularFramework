using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WebViewKit.WinForms
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WebViewKitControlSettings
    {
        /// <summary>
        /// 설정 변경 시 컨트롤에 즉시 반영하기 위한 이벤트입니다.
        /// </summary>
        internal event EventHandler? PropertyChanged;

        private bool _isCrawlingMode = false;
        [Category("Custom"), DefaultValue(false)]
        [Description(WebViewDescriptions.IsCrawlingMode)]
        public bool IsCrawlingMode
        {
            get => _isCrawlingMode;
            set { _isCrawlingMode = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        private bool _isPopupBlocked = false;
        [Category("Custom"), DefaultValue(false)]
        [Description(WebViewDescriptions.IsPopupBlocked)]
        public bool IsPopupBlocked
        {
            get => _isPopupBlocked;
            set { _isPopupBlocked = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        private string _firstUri = "about:blank";
        [Category("Custom"), DefaultValue("about:blank")]
        [Description(WebViewDescriptions.FirstUri)]
        public string FirstUri
        {
            get => _firstUri;
            set { _firstUri = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        private bool _areDefaultScriptDialogsEnabled = true;
        [Category("Browser"), DefaultValue(true)]
        [Description(WebViewDescriptions.AreDefaultScriptDialogsEnabled)]
        public bool AreDefaultScriptDialogsEnabled
        {
            get => _areDefaultScriptDialogsEnabled;
            set { _areDefaultScriptDialogsEnabled = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        private bool _isStatusBarEnabled = false;
        [Category("Browser"), DefaultValue(false)]
        [Description(WebViewDescriptions.IsStatusBarEnabled)]
        public bool IsStatusBarEnabled
        {
            get => _isStatusBarEnabled;
            set { _isStatusBarEnabled = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        private bool _areDevToolsEnabled = true;
        [Category("Browser"), DefaultValue(true)]
        [Description(WebViewDescriptions.AreDevToolsEnabled)]
        public bool AreDevToolsEnabled
        {
            get => _areDevToolsEnabled;
            set { _areDevToolsEnabled = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        private bool _isWebMessageEnabled = true;
        [Category("Browser"), DefaultValue(true)]
        [Description(WebViewDescriptions.IsWebMessageEnabled)]
        public bool IsWebMessageEnabled
        {
            get => IsWebMessageEnabled;
            set { IsWebMessageEnabled = value; PropertyChanged?.Invoke(this, EventArgs.Empty); }
        }

        /// <summary>
        /// 속성창에 표시될 요약 텍스트입니다.
        /// </summary>
        public override string ToString() => IsCrawlingMode ? "Crawling Mode (Active)" : "Normal Mode";
    }
}
