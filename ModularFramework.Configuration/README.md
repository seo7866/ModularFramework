# ModularFramework.Configuration

어트리뷰트 기반 설정 자동 등록 및 JSON 바인딩을 제공하는 설정 모듈입니다.

---

## 📌 개요

ModularFramework.Configuration은  
어셈블리 스캔을 통해 설정 모델을 자동 탐색하고  
JSON 설정 파일과 바인딩하여 DI 컨테이너에 등록합니다.

---

## 💡 설계 의도

설정 파일과 코드 바인딩을 분리하지 않고  
어트리뷰트 기반 선언만으로 설정 객체를 자동 등록합니다.

---

## ⚙ 주요 특징

- 어트리뷰트 기반 설정 자동 등록
- JSON 파일 자동 로딩
- FullName → Name → Root 우선순위 바인딩
- Web / Desktop 환경 공용 지원

---

## 🚀 사용 방법

---

## 1. 설정 모델 정의

```csharp

using ModularFramework.Configuration.Attributes;

[ConfigurationRegister("appsettings.json")]
public class AppSettingsOptions
{
    public int Value1 { get; set; }

    public string Value2 { get; set; }
}

```

## 2. JSON 바인딩 구조
1. FullName (Namespace 포함) => 2. Class Name => 3. Root

```JSON

{
  "ModularFramework.Configuration.Models.AppSettingsOptions": {
    "Value1": 10,
    "Value2": "FullName Binding"
  },

  "AppSettingsOptions": {
    "Value1": 20,
    "Value2": "ClassName Binding"
  },

  "Value1": 30,
  "Value2": "Root Binding"
}

```


## 3. Console / WPF / WinForms 사용

```csharp

using Microsoft.Extensions.Configuration;
using ModularFramework.Configuration.Extensions;

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

IConfiguration configuration = configBuilder.Build();

services.AddAutoConfigurationForApp(configuration, configBuilder);

```


## 4. Web (ASP.NET Core) 사용

```csharp

using ModularFramework.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoConfigurationForWeb(builder.Configuration);

var app = builder.Build();
app.Run();

```


## 🔄 동작 구조

Assembly Scan
        ↓
ConfigurationRegister Attribute 탐색
        ↓
JSON 파일 로드 (optional)
        ↓
설정 섹션 결정 (FullName → Name → Root)
        ↓
객체 바인딩 (Bind)
        ↓
Singleton 등록