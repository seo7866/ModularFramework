# ModularFramework

Attribute 기반 자동 등록과 런타임 어셈블리 스캔을 기반으로 구성된  
모듈형 .NET 프레임워크입니다.

---

## 📌 개요

ModularFramework는 기존 .NET 애플리케이션 개발에서 발생하는 다음 문제를 해결하기 위해 설계되었습니다.

- DI(Service Registration) 코드 반복
- 설정 및 매핑 코드의 분산
- 프로젝트 간 강한 결합
- 기능 추가 시 구조 확장 어려움
- 레거시 코드 유지보수 비용 증가

이를 해결하기 위해 Attribute 기반 선언 + 런타임 자동 구성 구조를 채택하였습니다.

---

## 💡 설계 목표

- 코드 기반 등록 최소화
- 모듈 단위 독립성 확보
- 기능 추가 시 기존 코드 수정 최소화
- 런타임 기반 자동 구성
- 역할 분리 설계 원칙

---

## 🧱 전체 구조

### Dependency Injection
Attribute 기반 DI 자동 등록 모듈

- Service / ViewModel 자동 등록
- Reflection 기반 어셈블리 스캔
- 수동 Service 등록 코드 제거

---

### Configuration
Attribute 기반 설정 자동 바인딩 모듈

- JSON 기반 설정 자동 로드
- 클래스 기반 Configuration 바인딩
- FullName → Name → Root 순서 매핑
- 런타임 설정 구성 자동화

---

### Data (QueryProvider)
파일 기반 SQL 실행 및 매핑 모듈

- 메서드 기반 SQL 매핑
- Namespace / Type / Method 기반 경로 구조
- Query 파일 외부 분리 관리
- 자동 Query Provider 등록

---

### WebViewKit
WebView2 기능을 API 형태로 제공하는 모듈

- Navigation / HTML / Download 기능 분리
- WinForms / WPF 동일 API 구조
- UI와 로직 분리
- WebView2 기능 비동기 API화

---

## 🧩 아키텍처 핵심 개념

### 1. Attribute 기반 선언 구조

```text

Class / Method / Property → Attribute 선언  
↓  
Runtime Scan  
↓  
자동 등록 / 바인딩

```

---

### 2. Runtime Assembly Scanning

애플리케이션 실행 시 어셈블리를 스캔하여  
필요한 모듈을 자동으로 구성

---

### 3. Loose Coupling 구조

- DI ↔ Configuration 독립
- Data ↔ WebView 독립
- Core 레이어 분리 유지

---

### 4. 기능 단위 모듈 설계

모든 기능은 프로젝트 단위가 아닌  
기능 단위 모듈로 분리

---

## 🚀 전체 동작 흐름

```text

Application Start  
↓  
Assembly Scan  
↓  
DI Auto Registration  
↓  
Configuration Auto Binding  
↓  
Query Provider Initialization  
↓  
WebViewKit Initialization  
↓  
Application Runtime

```

---

## ⚙ 사용 기술

- .NET 10
- Reflection / Attribute 기반 설계
- Microsoft.Extensions.DependencyInjection
- IConfiguration / ConfigurationBuilder
- WebView2
- ADO.NET / File-based SQL Mapping

---

## 🧠 설계 철학

- 등록 코드는 제거하고 선언으로 대체
- 기능은 모듈 단위로 분리
- 런타임에서 시스템이 스스로 구성
- UI와 로직 완전 분리

---

## 📂 Modules

- ModularFramework.DependencyInjection
- ModularFramework.Configuration
- ModularFramework.Data
- WebViewKit

---

## ✔ 특징 요약

- Attribute 기반 자동 구성
- 런타임 어셈블리 스캔
- 모듈 단위 구조 설계
- DI / Config / Data / UI 기능 분리
- WinForms / WPF / ASP.NET 공통 구조

---

## 📌 한줄 정의

ModularFramework는 Attribute 기반 선언과 런타임 자동 구성을 통해  
.NET 애플리케이션의 구조적 복잡도를 제거하는 모듈형 프레임워크입니다.