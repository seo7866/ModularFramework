using System;
using System.Collections.Generic;
using System.Text;

namespace WebViewKit
{
    public static class WebViewDescriptions
    {
        public const string IsCrawlingMode = "이미지, CSS, 폰트 등을 차단하여 로딩 속도를 최적화하는 모드입니다.";
        public const string FirstUri = "웹뷰 초기화 완료 후 자동으로 이동할 첫 페이지 주소입니다.";
        public const string AreDefaultScriptDialogsEnabled = "JavaScript의 alert, confirm 등 팝업 대화상자 허용 여부입니다.";
        public const string IsStatusBarEnabled = "하단 상태 표시줄(URL 미리보기 등) 표시 여부입니다.";
        public const string AreDevToolsEnabled = "F12 개발자 도구 활성화 여부입니다.";
        public const string IsWebMessageEnabled = "브라우저와 C# 간의 WebMessage 통신 허용 여부입니다.";
        public const string IsPopupBlocked = "새로운 창을 띄우는 팝업을 차단합니다.";
    }
}
