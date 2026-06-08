# ModularFramework.DependencyInjection

어트리뷰트 기반 선언형 DI 등록 및 자동 주입 프레임워크입니다.

---

## 💡 설계 철학

- DI 등록과 주입을 분리하지 않고 선언형으로 통합
- 런타임 reflection 최소화 + delegate 기반 주입
- 컴파일 타임(Analyzer) + 런타임 검증 이중 구조
- Host 기반 단일 진입점 구조

---

## 📌 개요

이 프레임워크는 다음을 자동 처리합니다:

- Assembly scanning
- Attribute 기반 DI 등록
- Interface → Implementation 매핑
- Property injection 수행
- Dependency graph 생성 및 캐싱

---

## ⚙ 주요 특징

- Attribute 기반 DI 등록
- ServiceLifetime 자동 적용
- Interface / Concrete 자동 매핑
- Property Injection 지원
- Constructor Injection 제한
- Analyzer 기반 규칙 강제
- Runtime dependency graph 생성
- IHost 기반 실행 구조

---

## 🚨 핵심 구조 (중요)

### ✔ DependencyInjectAttribute

- 모든 DI 속성의 기본 Attribute
- ResolveType 정보를 가짐
- Property / Parameter 대상

---

### ✔ DependencyServiceAttribute

DependencyInjectAttribute를 상속하며 다음 역할을 수행:

- DI 등록 대상 마킹
- 서비스 자동 등록
- 생성된 인스턴스의 property injection 수행 (핵심)
- 런타임에서 객체 생성 및 injection 트리거 역할

즉 단순 marker가 아니라:

> "등록 + 객체 생성 + property injection 수행 책임"

---

## 🚀 사용 방법

---

## 1. Host 생성 (단일 진입점)

```csharp

var host = DependencyInjectionHost.BuildAutoHost();

```

---

## 2. Host 생성 (옵션 포함)

```csharp

var host = DependencyInjectionHost.BuildAutoHost(
configure: services =>
{
	services.AddLogging();
},
	overrides: new Dictionary<Type, DependencyInjectionLifeTime>()
);

```

---

## ⚠ BuildAutoHost 내부 동작

1. Host 생성
2. ConfigureServices 실행
3. AddAutoRegister 실행
4. Dependency graph 분석
5. Cache 생성
6. ServiceProvider 확정
7. Runtime injection 활성화

---

## 3. 서비스 선언

### ✔ DependencyServiceAttribute (권장)

```csharp

[DependencyInjection(DependencyInjectionLifeTime.Singleton)]
public class MyService
{
}

```

---

### ✔ 인터페이스 포함 등록

```csharp

[DependencyInjection(DependencyInjectionLifeTime.Singleton, typeof(IMyService))]
public class MyService : IMyService
{
}

```

---

## 4. Property Injection

```csharp

[DependencyService(DependencyInjectionLifeTime.Singleton)]
public class Consumer
{
	[DependencyInject(typeof(MyService))]
	public IMyService Service { get; set; }
}

```

---

## ⚠ 규칙 (Analyzer enforced)

- constructor는 parameterless만 허용
- constructor injection 금지
- DependencyInject는 concrete type만 허용
- interface ambiguity는 Attribute로 해결
- DI 규칙 위반 시 컴파일 타임 오류

---

## 🧠 내부 동작 구조

1. Assembly Scan
2. Attribute 수집
3. Service 등록 분석
4. Interface mapping 생성
5. Dependency graph 생성
6. Activation cache 생성
7. delegate 기반 injection 수행

---

## 📦 구성 요소

- DependencyInjectionHost
  - IHost 기반 DI 초기화 Helper

- DependencyInjectionAttribute
  - DI 등록 Attribute Base

- DependencyServiceAttribute
  - DI 등록 및 Property Injection 지원

- DependencyInjectAttribute
  - 인터페이스 구현체 선택 Attribute

- AddAutoRegister()
  - Assembly Scan 기반 자동 등록 API

- DependencyInjectionFactory
  - 객체 생성 및 Injection 처리

- DependencyInjectionGraphBuilder
  - Dependency Graph 분석 및 Cache 생성

- Analyzer (MDI001)
  - DependencyService 생성자 사용 금지

---

## 🔥 핵심 특징

- reflection 최소화
- delegate 기반 injection
- deterministic DI resolution
- compile-time validation (Analyzer)
- Host 중심 구조
- registration + injection unified model

---

## 📌 권장 사용 방식

- 외부에서는 BuildAutoHost만 사용
- ServiceCollection 직접 사용 금지
- Attribute 기반 선언 통일
- DependencyServiceAttribute로 lifecycle + injection 동시에 처리