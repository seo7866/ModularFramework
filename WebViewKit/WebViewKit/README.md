# WebViewKit

WebView2 기반 브라우저 기능을 공통 추상화하여  
WinForms / WPF 환경에서 동일한 방식으로 사용할 수 있도록 제공하는 UI 통합 모듈입니다.

---

## 💡 설계 의도

WebView2 컨트롤은 플랫폼별(WPF / WinForms) API 차이와 초기화 방식 차이로 인해  
코드 중복 및 유지보수 비용이 발생합니다.

WebViewKit은 이를 해결하기 위해:

- 공통 브라우저 기능 추상화
- 플랫폼별 구현 분리
- 동일한 API 제공

을 목표로 설계되었습니다.

---

## 📌 개요

WebViewKit은 다음 기능을 공통 인터페이스로 제공합니다.

- 페이지 이동 (Navigate)
- HTML 추출
- 파일 다운로드 제어
- Script 실행
- WebMessage 통신
- Crawling 모드 제어

---

## ⚙ 특징

- WebView2 기반 공통 API
- WinForms / WPF 통합 설계
- 플랫폼 종속 코드 최소화
- 확장 가능한 구조

---

## 🧱 구조

```
WebViewKit (Core Interface)
        ↓
-------------------------
|                       |
WinForms                WPF
(WebView2Wrapper)       (WebView2Wrapper)
```

---