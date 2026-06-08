using Microsoft.Extensions.DependencyInjection;
using ModularFramework.DependencyInjection.Attributes;
using ModularFramework.DependencyInjection.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularFramework.DependencyInjection.Analysis
{
    static class DependencyInjectionGraphBuilder
    {
        internal static (Dictionary<Type, List<Type>> interfaceMap, 
            Dictionary<Type, ServiceLifetime> lifetimeMap,
            HashSet<Type> registeredTypes) AnalyzeServiceTypeMaps(this IServiceCollection services)
        {
            Dictionary<Type, List<Type>> interfaceMap = [];
            Dictionary<Type, ServiceLifetime> lifetimeMap = [];
            HashSet<Type> registeredTypes = [];

            foreach (var d in services)
            {
                Type impl = d.ImplementationType;
                // 1. 구체 클래스 타입 추론 (팩토리/인스턴스 등록 대응)
                if (impl == null && d.ServiceType.IsClass && !d.ServiceType.IsAbstract)
                {
                    impl = d.ServiceType;
                }
                // 팩토리 메서드(sp => new Service()) 형태로 등록되어 implementationType 추론이 불가능한 경우, 
                // ServiceType 자체가 구체 클래스라면 그것을 활용합니다.
                if (impl == null && d.ImplementationFactory != null && d.ServiceType.IsClass && !d.ServiceType.IsAbstract)
                {
                    impl = d.ServiceType;
                }

                if (impl == null)
                    continue;

                // 2. 구현체의 수명 주기 수집
                lifetimeMap[impl] = d.Lifetime;
                registeredTypes.Add(impl);
                // 3. 인터페이스 매핑 수집 (클래스 자기 자신은 제외)

                var v = impl.GetInterfaces();
                foreach (var i in impl.GetInterfaces())
                {
                    interfaceMap.TryAdd(i, []);
                    if (!interfaceMap[i].Contains(impl))
                        interfaceMap[i].Add(impl);
                }
            }

            return (interfaceMap, lifetimeMap, registeredTypes);
        }

        internal static void AnalyzeDependencyBundles(List<Type> dependencyServices, 
            Dictionary<Type, List<Type>> interfaceImplementations, 
            HashSet<Type> registeredTypes)
        {
            foreach (var serviceType in dependencyServices)
            {
                var ctors = serviceType.GetConstructors(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);
                bool isValid = ctors.Length == 1 && ctors[0].GetParameters().Length == 0;
                if (!isValid)
                {
                    throw new InvalidOperationException(
                        $"[DependencyService] {serviceType.FullName}는 생성자 DI를 사용할 수 없습니다.\n\n" +
                                            "규칙:\n" +
                                            "- parameterless constructor만 허용됩니다.\n" +
                                            "- 모든 의존성은 property injection으로 처리됩니다."
                    );
                }

                List<InjectPropertyInfo> bundleProperties = [];
                //internal or public 인스턴스 프로퍼티에서 SetMethod, GetMethod가 있는 프로퍼티만 추출
                var properties = serviceType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p =>
                        p.SetMethod != null &&
                        p.GetMethod != null &&
                        (p.SetMethod.IsPublic || p.SetMethod.IsAssembly) &&
                        (p.GetMethod.IsPublic || p.GetMethod.IsAssembly)
                    );

                foreach (var prop in properties)
                {
                    bool registered = true;
                    var injectAttr = prop.GetCustomAttribute<DependencyInjectAttribute>();
                    Type resolveType = null;
                    if (prop.PropertyType.IsInterface)
                    {
                        // 1. 인터페이스 매핑 확인
                        if (!interfaceImplementations.TryGetValue(prop.PropertyType, out var implementations) || implementations.Count == 0)
                        {
                            throw new InvalidOperationException(
                                $"[DependencyBundle] 인터페이스 '{prop.PropertyType.FullName}'에 대한 구현체를 찾을 수 없습니다.\n\n" +
                                $"발생 위치: {serviceType.FullName}.{prop.Name}\n\n" +
                                "가능한 원인:\n" +
                                "- 해당 인터페이스를 구현한 클래스가 DI에 등록되지 않았습니다.\n" +
                                "- DependencyInjectionAttribute가 누락되어 자동 등록 대상이 아닙니다.\n" +
                                "- 외부에서 Factory 방식(sp => new ...)으로 등록되어 분석 대상에 포함되지 않았습니다.\n\n" +
                                "해결 방법:\n" +
                                "- 구현 클래스를 DependencyInjectionAttribute로 등록하세요.\n" +
                                "- 또는 DependencyInjectAttribute를 사용하여 명시적으로 매핑을 지정하세요."
                            );
                        }

                        // 2. 매핑 전략 (1:1 자동 혹은 어트리뷰트 힌트)
                        if (implementations.Count == 1)
                        {
                            resolveType = implementations[0];
                        }
                        else if (injectAttr != null && implementations.Contains(injectAttr.ResolveType))
                        {
                            resolveType = injectAttr.ResolveType;
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"[DependencyBundle] 인터페이스 '{prop.PropertyType.FullName}'에 대해 여러 구현체가 존재하지만 선택 규칙이 정의되지 않았습니다.\n\n" +
                                $"발생 위치: {serviceType.FullName}.{prop.Name}\n\n" +
                                $"등록된 구현체: {string.Join(", ", implementations.Select(x => x.FullName))}\n\n" +
                                "가능한 원인:\n" +
                                "- 동일 인터페이스를 구현한 클래스가 2개 이상 존재합니다.\n" +
                                "- DependencyInjectAttribute로 특정 구현체가 명시되지 않았습니다.\n\n" +
                                "해결 방법:\n" +
                                "- [DependencyInject]를 사용하여 사용할 구현체를 명확히 지정하세요.\n" +
                                "- 또는 DI 등록 구조에서 해당 인터페이스 구현체를 단일화하세요."
                            );
                        }
                    }
                    else
                    {
                        // 인터페이스가 아닌 구체 클래스 타입인 경우 자기 자신을 ResolveType으로 설정
                        resolveType = prop.PropertyType;
                        registered = registeredTypes.Contains(prop.PropertyType);
                    }

                    // PropertyInfo를 포함하여 캐시에 추가
                    if (registered)
                        bundleProperties.Add(new(prop, resolveType));
                }

                if (!DependencyInjectionFactory.ServiceActivationCache.TryAdd(serviceType, new([.. bundleProperties])))
                {
                    throw new InvalidOperationException($"{serviceType.FullName} BundleCache 등록 중복");
                }
            }
        }
    }
}