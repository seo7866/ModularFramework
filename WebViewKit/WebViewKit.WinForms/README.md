# WebViewKit.WinForms

WinForms 환경에서 WebView2를 추상화하여  
공통 WebViewKit 인터페이스를 구현한 모듈입니다.

---

## 💡 개요

WinForms WebView2 컨트롤을 기반으로  
WebViewKit.Core 기능을 WinForms 방식으로 구현합니다.

---

## ⚙ 주요 기능

- 페이지 이동 (Navigate)
- HTML Source 가져오기
- JavaScript 실행
- 파일 다운로드 처리
- WebMessage 통신
- DevTools 제어
- StatusBar 제어

---

## 🧠 특징

- WebView2 WinForms Wrapper
- Core API 1:1 매핑
- UI 이벤트 캡슐화
- 비동기 작업 지원

---

## 🚀 사용 방식

WinForms에서는 WebViewKit을 통해 동일한 API로 접근합니다.

(구체 구현은 Core 인터페이스 기반)

---

## 📌 구조 역할

- WebView2 WinForms 컨트롤 래핑
- Core 기능 구현체
- 이벤트 브릿지 역할