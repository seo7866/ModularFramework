using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebViewKit
{
    public sealed class NavigationResult
    {
        public bool IsSuccess { get; init; }
        public bool IsTimeout { get; init; }
        public CoreWebView2NavigationCompletedEventArgs EventArgs { get; init; }
        public Exception Error { get; init; }
    }
}