# WebViewKit.WinForms

WinForms 환경에서 WebView2 기능을 단순화하여 사용할 수 있도록 제공하는 래퍼입니다.

---

## 📌 개요

WebViewKit.WinForms는 WebViewKit.Core 기능을 WinForms 환경에서  
직접 사용하기 쉽게 감싼 래퍼 라이브러리입니다.

초기화 및 복잡한 WebView2 이벤트 처리를 내부에서 처리하여  
개발자는 기능 호출에만 집중할 수 있도록 설계되었습니다.

---

## 💡 설계 의도

WinForms 환경에서 WebView2 사용 시 발생하는 다음 문제를 해결합니다:

- 초기화 코드 반복
- 이벤트 등록/해제 복잡도
- 비동기 처리 구조 분산

이를 해결하기 위해 Core 기능을 직접 노출하지 않고  
단순 API 형태로 제공합니다.

---

## ⚙ 제공 기능

WinForms에서는 Core 기능을 다음 형태로 제공합니다:

- NavigateWithAwaitAsync
- DownloadFileAsync
- GetCurrentHtmlAsync

---

## 🧩 구조 특징

- Core 기능 1:1 래핑 구조
- WebView2 초기화 자동 처리
- UI 이벤트 캡슐화
- WinForms 환경 최적화
- Core 의존 구조 유지