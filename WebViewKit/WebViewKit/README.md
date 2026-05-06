# WebViewKit

WebView2 기능을 기능 단위 API로 분리하여 제공하는 Core 라이브러리입니다.

---

## 📌 개요

WebViewKit.Core는 WebView2를 UI 컨트롤이 아닌 기능 중심 API 계층으로 분리하여  
Navigation, HTML 추출, 파일 다운로드 기능을 독립적으로 사용할 수 있도록 설계되었습니다.

이를 통해 WebView2 기반 로직을 UI와 분리하여 재사용성과 유지보수성을 확보합니다.

---

## 💡 설계 의도

WebView2는 기본적으로 UI 컨트롤 중심 구조로 설계되어 있어  
기능 재사용 및 테스트가 어려운 문제가 있습니다.

이를 해결하기 위해 WebViewKit은 다음 기준으로 설계되었습니다:

- 기능 단위 분리 (Navigation / HTML / Download)
- UI 의존성 제거
- 결합도 최소화
- 필요한 기능만 선택적으로 사용 가능

---

## ⚙ 주요 기능

### NavigationWithAwaitAsync
페이지 이동 완료 여부를 Task 기반으로 제어할 수 있는 기능입니다.

NavigationCompleted 이벤트를 비동기로 래핑하여  
페이지 이동 완료 시점을 명확하게 제어할 수 있습니다.

→ 데이터 로딩 / 크롤링 흐름 제어에 사용

---

### GetCurrentHtmlAsync
현재 WebView2에 로드된 HTML을 반환합니다.

JavaScript 실행 결과를 기반으로 DOM을 추출하며  
페이지 이동 이후 데이터 파싱 및 크롤링에 사용됩니다.

---

### DownloadFileAsync
브라우저 다운로드 UI를 거치지 않고  
지정된 경로로 파일을 직접 저장하는 기능입니다.

→ 서버 기반 자동 다운로드 처리에 사용

---

## 🧩 설계 구조

- Navigation / Script / Download 기능 분리
- 이벤트 기반 로직 캡슐화
- 상태 기반 Navigation 결과 처리
- UI 계층과 완전히 분리된 구조