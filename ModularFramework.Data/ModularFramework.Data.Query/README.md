# ModularFramework.Data.Query

텍스트 기반 SQL 파일을 관리하고, 메서드 정보를 기반으로 쿼리를 자동 매핑하여 실행할 수 있도록 지원하는 Query Provider 모듈입니다.

---

## 💡 설계 의도

SQL을 코드에서 분리하여 관리하고,  
쿼리 변경 시 코드 수정 없이 유지보수가 가능하도록 설계되었습니다.

또한, 별도의 설정 없이도 동작하도록 컨벤션 기반 구조를 제공하여  
사용자의 실수를 최소화하는 것을 목표로 합니다.

---

## 📌 개요

ModularFramework.Data.Query는  
외부 SQL 파일을 로드하고, 메서드 정보를 기반으로 자동 매핑하여  
애플리케이션에서 사용할 수 있도록 합니다.

---

## ⚙ 주요 특징

- 파일 기반 SQL 관리
- QueryProvider 자동 등록 (DI)
- 기본 경로 자동 설정 (옵션 생략 가능)
- 컨벤션 기반 쿼리 매핑 (Attribute 없이 사용 가능)
- Attribute 기반 경로 지정 지원
- async / await 및 WPF RelayCommand 지원
- 런타임 쿼리 로딩

---

## 🚀 사용 방법

---

## 1. 서비스 등록

```csharp

using ModularFramework.Data.Query.Extensions;

services.AddQueryProvider();

※ 옵션을 생략하면 기본 경로 `/Queries`를 사용합니다.

```

또는 사용자 경로 지정:

```csharp

using ModularFramework.Data.Query.Extensions;

services.AddQueryProvider(options =>
{
    options.QueryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CustomQueries");
});
```

## 2. 쿼리 매핑 방식 (중요)

QueryProvider는 두 가지 방식으로 쿼리를 찾습니다.

---

## ✔ 1) Attribute 기반 매핑 (명시적 방식)

메서드에 어트리뷰트를 지정하여 쿼리 경로를 직접 지정할 수 있습니다.

```csharp

[QueryPath("User/GetUser.sql")]
public async Task<User> GetUser(int id)
{
    ...
}

```

또는:

```csharp

[QueryPath("CustomPath/MyQuery.sql")]
public async Task<List<User>> GetUsers()
{
    ...
}

```

👉 이 경우 지정한 경로를 그대로 사용합니다.

---

## ✔ 2) Convention 기반 매핑 (자동 방식)

어트리뷰트를 사용하지 않으면  
메서드 정보를 기반으로 경로가 자동 생성됩니다.

---

### 매핑 규칙

```text

Namespace → 폴더 경로
Class Name → 폴더
Method Name → 파일명

```

---

### 경로 생성 방식

```text

{Namespace(Assembly 제외)}/{ClassName}/{MethodName}.sql

```

---

### 예시

```text

Namespace: MyApp.Services
Class: UserService
Method: GetUser

```

→ 생성 경로:

```text

/Services/UserService/GetUser.sql

```

---

### 내부 동작 개념

```text

AssemblyName 제거
   ↓
Namespace → 경로 변환 ('.' → '/')
   ↓
ClassName 추가
   ↓
MethodName.sql 추가

```

---

## 3. 사용 방법 (쿼리 실행)

DI로 등록된 서비스 내에서 메서드 기준으로 쿼리를 자동으로 매핑하여 사용할 수 있습니다.

---

### ✔ 예시 서비스

```csharp

using ModularFramework.Data.Query.Interfaces;

public class UserService
{
    private readonly IQueryProvider _queryProvider;

    public UserService(IQueryProvider queryProvider)
    {
        _queryProvider = queryProvider;
    }

    public async Task<User> GetUser(int id)
    {
        return await _queryProvider.ExecuteAsync<User>(MethodBase.GetCurrentMethod(), new { Id = id });
    }
}
```

---

### ✔ 핵심 포인트

```text

MethodBase.GetCurrentMethod()
   ↓
메서드 정보 기반 쿼리 탐색
   ↓
Attribute 또는 Convention 경로 결정
   ↓
SQL 파일 로드
   ↓
실행

```

---

### ✔ WPF / RelayCommand 예시

```csharp

public IRelayCommand LoadUserCommand => new RelayCommand(async () =>
{
    await _queryProvider.ExecuteAsync<User>(MethodBase.GetCurrentMethod(), new { Id = SelectedId });
});

```

---

### ✔ 특징

- 메서드 기준 자동 매핑
- Attribute / Convention 동일 방식 사용
- DI 기반으로 주입
- 호출 위치만으로 SQL 결정


## ⚡ Async 및 WPF 지원

비동기 메서드 및 WPF RelayCommand 환경에서도  
정확한 메서드 기반으로 쿼리를 찾을 수 있도록 처리되어 있습니다.

---

### 문제

async 메서드는 컴파일 시 상태 머신으로 변환됩니다.

```text

GetUser() → MoveNext()

```

---

### 해결 방식

```text

MoveNext()
   ↓
AsyncStateMachineAttribute 분석
   ↓
원본 메서드 추적
   ↓
실제 메서드명 사용

```

---

### 효과

- async / await 환경 정상 동작
- WPF MVVM 패턴 호환
- 추가 설정 불필요

---

## ⚠ 주의 사항

쿼리 파일 경로가 존재하지 않으면 예외가 발생합니다.

```text

Query directory not found. Please check QueryProviderOptions.

```

---

## 🔧 설정 팁 (중요)

쿼리 파일이 출력 디렉터리에 복사되지 않으면  
런타임에서 파일을 찾을 수 없습니다.

프로젝트 파일(.csproj)에 아래 설정을 추가하세요:

```text

<ItemGroup>
  <None Update="Queries\**\*">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>

```

---

## 💡 권장 구조

```text

/Queries
    /UserService
        GetUser.sql
        InsertUser.sql
    /OrderService
        GetOrder.sql

```

---

## 🔄 동작 흐름

```text

Application Start
   ↓
QueryProvider 등록
   ↓
QueryPath 결정
   ↓
메서드 호출
   ↓
Attribute 존재 여부 확인
   ↓
  ├─ 있음 → 지정 경로 사용
   │
   └─ 없음 → Convention 기반 경로 생성
   ↓
실제 메서드 추적 (async 대응)
   ↓
파일 로드
   ↓
사용

```