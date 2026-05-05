# ModularFramework

어트리뷰트 기반 의존성 주입과 런타임 어셈블리 스캔을 활용한 모듈형 .NET 프레임워크입니다.

---

## 📌 프로젝트 개요

ModularFramework는 기존 .NET 애플리케이션 개발에서 발생하는 다음 문제를 해결하기 위해 설계되었습니다.

- DI(Service Registration) 위치 추적의 어려움
- 반복적인 서비스 등록 코드
- 프로젝트 간 강한 결합
- 설정 및 매핑 코드의 분산

이를 해결하기 위해 **어트리뷰트 기반 선언 + 런타임 자동 등록 구조**를 채택했습니다.

---

## 🧱 전체 구조

- Core  
  → 어셈블리 스캔 및 타입 로딩

- DependencyInjection  
  → 어트리뷰트 기반 서비스 자동 등록

- Configuration  
  → JSON 기반 설정 로딩 및 싱글톤 관리

- Data  
  → QueryProvider 기반 데이터 접근 계층

- WebViewKit  
  → WinForms / WPF 기반 WebView2 추상화 컨트롤

---

## ⚙ 핵심 특징

- 어트리뷰트 기반 DI 등록
- 런타임 어셈블리 스캔 구조
- 모듈 단위 분리 설계
- WinForms / WPF 공통 WebViewKit 제공
- QueryProvider 기반 데이터 접근 추상화

---

## 💡 설계 방향

이 프레임워크는 “명시적 등록 코드 제거”와 “사용자 경험 단순화”를 목표로 합니다.

> 개발자는 “어디서 등록했는지”를 추적하는 대신  
> “어트리뷰트 선언만으로 기능을 사용”할 수 있도록 설계되었습니다.

---

## 🚧 상태

- Active development (WIP)
- 구조 안정화 단계

---

## 📂 Modules

각 모듈은 독립적으로 사용 가능하도록 설계되어 있습니다.
