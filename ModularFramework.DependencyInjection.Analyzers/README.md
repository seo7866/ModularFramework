# ModularFramework.DependencyInjection.Analyzers

ModularFramework.DependencyInjection 전용 Roslyn Analyzer입니다.

DependencyInjection 프레임워크의 설계 규칙을 컴파일 타임에 검증하여 런타임 오류를 사전에 방지합니다.

---

## 💡 목적

ModularFramework.DependencyInjection은 Property Injection 기반으로 동작합니다.

따라서 DependencyService로 선언된 클래스는 생성자 DI를 사용할 수 없습니다.

Analyzer는 이러한 규칙 위반을 컴파일 단계에서 검출하여 개발자가 즉시 수정할 수 있도록 지원합니다.

---

## ⚙ 주요 기능

- DependencyService 클래스 분석
- 생성자 사용 여부 검사
- 컴파일 타임 오류 제공
- 런타임 DI 오류 사전 차단

---

## 📌 지원 규칙

### MDI001

DependencyService는 생성자를 가질 수 없습니다.

#### 잘못된 예

```csharp

[DependencyService(DependencyInjectionLifeTime.Singleton)]
public class UserService
{
	public UserService()
	{
	}
}

```

결과:


MDI001: UserService는 생성자를 정의할 수 없습니다.


#### 올바른 예

```csharp

[DependencyService(DependencyInjectionLifeTime.Singleton)]
public class UserService
{
}

```

---

## 🧠 검사 대상

다음 조건을 모두 만족하는 클래스:


[DependencyService(...)]
public class ExampleService
{
}


Analyzer는 DependencyServiceAttribute가 선언된 클래스만 검사합니다.

---

## 🔥 설계 철학

ModularFramework.DependencyInjection은 다음 원칙을 따릅니다.

- Constructor Injection 금지
- Property Injection 사용
- 선언 기반 DI 구성
- 런타임 예외 최소화
- 컴파일 타임 검증 우선

---

## 📦 패키지 구성


ModularFramework.DependencyInjection
└─ ModularFramework.DependencyInjection.Analyzers


DependencyInjection 패키지를 참조하면 Analyzer도 함께 적용됩니다.

---

## 📌 참고

Analyzer는 Roslyn 기반으로 동작하며 Visual Studio 및 MSBuild 빌드 과정에서 자동 실행됩니다.

Runtime 동작에는 영향을 주지 않으며 컴파일 단계에서만 검증을 수행합니다.