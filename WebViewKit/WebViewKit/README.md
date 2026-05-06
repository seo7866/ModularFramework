# WebViewKit

WebView2 기반 공통 기능을 제공하는 핵심 라이브러리입니다.

---

## 📌 개요

WebViewKit은 WinForms / WPF 환경에서 WebView2를 사용할 때 발생하는  
초기화, 비동기 처리, 이벤트 관리 복잡도를 제거하기 위해 설계된 라이브러리입니다.

WebView2를 컨트롤 기반이 아닌 **확장 메서드 기반 API 구조**로 제공하여  
일관된 방식으로 Navigation, Download, Script 실행 기능을 사용할 수 있도록 합니다.

---

## ⚙ 핵심 특징

- WebView2 초기화 표준화
- Navigation / Download / Script 비동기 API 제공
- WinForms / WPF 동일 동작 구조
- 이벤트 기반 로직 캡슐화
- 상태 기반 Navigation 처리 모델
- 불필요한 동기화(Semaphore) 제거 구조

---

## 🧩 구성

### WebViewKit.Core
- WebView2 확장 메서드 제공
- Navigation / Download / Script 핵심 로직 구현
- 상태 기반 WebView 처리 구조

### WebViewKit.WinForms
- WinForms WebView2 래퍼 제공
- Core 기능을 WinForms 환경에 맞게 연결

### WebViewKit.WPF
- WPF WebView2 래퍼 제공
- MVVM 구조 대응
- Command 기반 확장 가능

---

## 🔥 설계 방향

- WebView2 초기화 책임은 플랫폼 계층에서 처리
- Core는 기능 로직만 담당
- UI 계층은 최소한의 래핑만 수행
- Navigation 결과는 상태 기반 처리
- 동기화가 아닌 이벤트 기반 흐름 유지