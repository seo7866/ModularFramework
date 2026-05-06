# WebViewKit.WPF

WPF 환경에서 WebView2를 MVVM 구조에 맞게 사용할 수 있도록 제공하는 래퍼 라이브러리입니다.

---

## 📌 개요

WebViewKit.WPF는 WPF 기반 WebView2 컨트롤을 래핑하여  
MVVM 패턴에서도 WebView2 기능을 자연스럽게 사용할 수 있도록 설계되었습니다.

Command, Binding 구조에서도 WebView2 기능을 직접 사용할 수 있습니다.

---

## ⚙ 주요 특징

- WebView2 초기화 자동 처리
- MVVM 구조 대응
- ICommand 기반 연동 가능
- Core API 직접 사용 가능
- 비동기 Navigation / Script 지원
- WPF Dispatcher 환경 고려

---

## 🧩 구조

- WebView2 컨트롤 래핑
- WebViewKit.Core 기능 기반 동작
- MVVM 친화 구조
- ViewModel 중심 확장 가능

---

## 💡 설계 목적

WPF 환경에서 WebView2를 ViewModel과 분리된 상태로 안전하게 사용하면서도  
Command 기반 구조에서 자연스럽게 연동될 수 있도록 하는 것이 목적입니다.