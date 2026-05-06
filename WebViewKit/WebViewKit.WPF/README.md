# WebViewKit.WPF

WPF 환경에서 WebView2를 추상화하여  
WebViewKit.Core 인터페이스를 구현한 모듈입니다.

---

## 💡 개요

WPF WebView2는 WinForms와 구조가 다르기 때문에  
초기화 및 이벤트 처리 방식이 다릅니다.

WebViewKit.WPF는 이를 통합하여 동일 API를 제공합니다.

---

## ⚙ 주요 기능

- Navigate (페이지 이동)
- HTML 추출
- Script 실행
- WebMessage 통신
- Download 제어
- DevTools 활성화
- Crawling 모드 지원

---

## 🧠 특징

- WPF WebView2 래핑
- Core 인터페이스 구현
- MVVM 환경 대응
- Command 기반 연동 가능
- 비동기 초기화 지원

---

## 🚀 설계 포인트

- WPF 종속 코드 최소화
- ViewModel과 분리된 구조
- WinForms와 동일 API 제공

---

## 📌 구조 역할

- WebView2 WPF 컨트롤 래핑
- Core 기능 구현
- MVVM 친화 구조