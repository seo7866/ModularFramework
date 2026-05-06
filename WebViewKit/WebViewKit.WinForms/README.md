# WebViewKit.WinForms

WinForms 환경에서 WebView2 사용을 단순화하기 위한 래퍼 라이브러리입니다.

---

## 📌 개요

WebViewKit.WinForms는 WinForms 기반 WebView2 컨트롤을 래핑하여  
초기화 및 이벤트 처리를 내부에서 자동으로 수행합니다.

개발자는 WebViewKit.Core에서 제공하는 기능만 호출하여  
WebView2 기능을 사용할 수 있습니다.

---

## ⚙ 주요 특징

- WebView2 초기화 자동 처리
- Core API 1:1 매핑 구조
- 이벤트 처리 캡슐화
- 비동기 Navigation / Download 지원
- WinForms 환경 최적화

---

## 🧩 구조

- WebView2 컨트롤 래핑
- WebViewKit.Core 기능 직접 사용
- UI 이벤트 최소화 구조
- WinForms 특화 이벤트 처리

---

## 💡 설계 목적

WinForms 환경에서 WebView2 초기화 및 이벤트 처리 코드를 제거하고  
핵심 기능만 사용하는 구조를 제공하는 것이 목적입니다.