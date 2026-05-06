# WebViewKit.WPF

WPF 환경에서 MVVM 구조에서도 WebView2를 쉽게 사용할 수 있도록 제공하는 래퍼입니다.

---

## 📌 개요

WebViewKit.WPF는 WebViewKit.Core 기능을 WPF 환경에서  
MVVM 패턴과 함께 사용할 수 있도록 설계된 래퍼입니다.

ViewModel 중심 구조에서도 WebView2 기능을 자연스럽게 사용할 수 있습니다.

---

## 💡 설계 의도

WPF 환경에서는 UI와 로직 분리가 필수이기 때문에  
WebView2를 직접 컨트롤하는 방식은 구조적으로 적합하지 않습니다.

이를 해결하기 위해 다음 기준으로 설계되었습니다:

- MVVM 구조 대응
- Command 기반 연동 가능
- Core 기능 직접 호출 제거
- UI와 로직 완전 분리

---

## ⚙ 제공 기능

WPF 래퍼는 WinForms와 동일한 API를 제공합니다:

- NavigateWithAwaitAsync
- DownloadFileAsync
- GetCurrentHtmlAsync

---

## 🧩 구조 특징

- WebView2 컨트롤 래핑
- Core 기능 직접 연결 구조
- MVVM 친화 구조
- ICommand 확장 가능
- Dispatcher 환경 고려