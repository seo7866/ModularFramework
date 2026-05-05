using Microsoft.Extensions.DependencyInjection;
using ModularFramework.Core.Resolvers;
using ModularFramework.DependencyInjection.Attributes;
using ModularFramework.DependencyInjection.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularFramework.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 어셈블리 스캔을 통한 서비스 자동 의존성 주입(DI) 등록
        /// </summary>
        /// <remarks>
        /// 1. 실행 중인 어셈블리를 기준으로 참조된 모든 프로젝트를 탐색합니다.
        /// 2. DependencyInjectionAttribute가 부여된 클래스를 자동으로 찾아 컨테이너에 등록합니다.
        /// 3. 개발자가 매번 Startup이나 Program.cs를 수정할 필요 없이 선언적(Attribute)으로 관리할 수 있게 합니다.
        /// </remarks>
        /// <param name="services">DI 컨테이너</param>
        /// <param name="overrides">코드 레벨에서 특정 타입의 수명 주기를 강제로 덮어쓰기 위한 딕셔너리</param>
        /// <returns>구성이 완료된 IServiceCollection</returns>
        public static IServiceCollection AddAutoRegister(this IServiceCollection services, Dictionary<Type, DependencyInjectionLifeTime> overrides = null)
        {
            overrides ??= [];
            var currentLibrary = Assembly.GetExecutingAssembly();
            var assemblies = AssemblyDependencyResolver.GetDependentAssemblies(currentLibrary);
            var types = assemblies.SelectMany(a => a.GetTypes())
                                  .Where(t => t.IsClass && !t.IsAbstract);

            foreach (Type type in types)
            {
                // 수명 주기 결정을 위한 임시 변수 (기본값으로 Ignore 설정 가능)
                DependencyInjectionLifeTime finalLifeTime = DependencyInjectionLifeTime.Ignore;
                bool isSet = false;
                Type[] interfaces = null;

                // 1. 딕셔너리(오버라이드) 확인 - 최우선
                if (overrides.TryGetValue(type, out DependencyInjectionLifeTime overrideLifeTime))
                {
                    finalLifeTime = overrideLifeTime;
                    isSet = true;
                }

                // 2. 어트리뷰트 확인
                DependencyInjectionAttribute attr = type.GetCustomAttribute<DependencyInjectionAttribute>();
                if (attr != null)
                {
                    // 딕셔너리 설정이 없을 때만 어트리뷰트 값 사용
                    if (!isSet)
                    {
                        finalLifeTime = attr.DependencyInjectionLifeTimeType;
                        isSet = true;
                    }
                    interfaces = attr.InterfaceTypes;
                }

                // 3. 등록 여부 결정 (설정이 없거나 Ignore면 패스)
                if (!isSet || finalLifeTime == DependencyInjectionLifeTime.Ignore)
                    continue;

                // 4. 서비스 수명 주기 매핑
                ServiceLifetime lifetime = finalLifeTime switch
                {
                    DependencyInjectionLifeTime.Singleton => ServiceLifetime.Singleton,
                    DependencyInjectionLifeTime.Scoped => ServiceLifetime.Scoped,
                    DependencyInjectionLifeTime.Transient => ServiceLifetime.Transient,
                    _ => ServiceLifetime.Transient
                };

                // 5. 본체 등록
                services.Add(new ServiceDescriptor(type, type, lifetime));

                // 6. 인터페이스 등록
                if (interfaces != null && interfaces.Length > 0)
                {
                    foreach (Type i in interfaces)
                    {
                        // 본체 인스턴스를 공유하도록 팩토리 방식으로 등록
                        services.Add(new ServiceDescriptor(i, sp => sp.GetRequiredService(type), lifetime));
                    }
                }
            }

            return services;
        }
    }
}