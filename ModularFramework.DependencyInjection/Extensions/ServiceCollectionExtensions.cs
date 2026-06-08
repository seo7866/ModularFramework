using Microsoft.Extensions.DependencyInjection;
using ModularFramework.Core.Resolvers;
using ModularFramework.DependencyInjection.Analysis;
using ModularFramework.DependencyInjection.Attributes;
using ModularFramework.DependencyInjection.Enums;
using ModularFramework.DependencyInjection.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.AccessControl;
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
        public static IServiceCollection AddAutoRegister(
            this IServiceCollection services,
            Dictionary<Type, DependencyInjectionLifeTime> overrides = null)
        {
            overrides ??= [];

            var currentLibrary = Assembly.GetExecutingAssembly();
            var assemblies = AssemblyDependencyResolver.GetDependentAssemblies(currentLibrary);

            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract);

            List<Type> dependencyServices = [];

            // 기존 DI 상태 분석 (수동 등록 포함)
            var (interfaceMap, lifetimeMap, registeredTypes) = services.AnalyzeServiceTypeMaps();

            foreach (var type in types)
            {
                DependencyInjectionLifeTime lifeTime = DependencyInjectionLifeTime.Ignore;
                bool hasLifeTime = false;
                Type[] interfaces = null;
                string key = string.Empty;

                #region 1. 수동 override 우선
                if (overrides.TryGetValue(type, out var overrideLifeTime))
                {
                    lifeTime = overrideLifeTime;
                    hasLifeTime = true;
                }
                #endregion

                #region 2. Attribute 기반 설정
                var attr = type.GetCustomAttribute<DependencyInjectionAttribute>();
                if (attr != null)
                {
                    if (!hasLifeTime)
                    {
                        lifeTime = attr.DependencyInjectionLifeTimeType;
                        hasLifeTime = true;
                    }

                    interfaces = attr.InterfaceTypes;
                    key = attr.Key;
                }
                #endregion

                #region 3. 등록 대상 아님
                if (!hasLifeTime || lifeTime == DependencyInjectionLifeTime.Ignore)
                    continue;
                #endregion

                var lifetime = lifeTime switch
                {
                    DependencyInjectionLifeTime.Singleton => ServiceLifetime.Singleton,
                    DependencyInjectionLifeTime.Scoped => ServiceLifetime.Scoped,
                    DependencyInjectionLifeTime.Transient => ServiceLifetime.Transient,
                    _ => ServiceLifetime.Transient
                };

                #region 4. 수동 등록 보호 (핵심)
                // 이미 수동으로 등록된 경우 → 자동 등록 절대 금지
                if (services.Any(d => d.ServiceType == type || d.ImplementationType == type))
                {
                    continue; // 철학: manual > auto
                }
                #endregion

                #region 5. 클래스 등록 및 6. 인터페이스 등록 통합 (Keyed 및 DependencyService 완전 대응)

                bool isDependencyService = attr is DependencyServiceAttribute;

                // ======================================================
                // 1. IMPLEMENTATION 등록 (단일화)
                // ======================================================

                if (isDependencyService)
                {
                    dependencyServices.Add(type);

                    services.Add(new ServiceDescriptor(
                        type,
                        sp => DependencyInjectionFactory.CreateDependencyService(sp, type),
                        lifetime));
                }
                else
                {
                    if (!string.IsNullOrEmpty(key))
                        services.AddKeyed(lifetime, type, key, type);
                    else
                        services.Add(new ServiceDescriptor(type, type, lifetime));
                }

                // ======================================================
                // 2. INTERFACE 등록
                // ======================================================

                if (interfaces != null && interfaces.Length > 0)
                {
                    foreach (var i in interfaces)
                    {
                        interfaceMap.TryAdd(i, []);

                        if (interfaceMap[i].Contains(type))
                            continue;

                        if (isDependencyService)
                        {
                            services.Add(new ServiceDescriptor(
                                i,
                                sp => sp.GetRequiredService(type),
                                lifetime));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(key))
                            {
                                services.AddKeyed(lifetime, i, key, type);
                            }
                            else
                            {
                                services.Add(new ServiceDescriptor(
                                    i,
                                    sp => sp.GetRequiredService(type),
                                    lifetime));
                            }
                        }

                        interfaceMap[i].Add(type);
                    }
                }

                #endregion

                lifetimeMap[type] = lifetime;
                registeredTypes.Add(type);
            }

            // DependencyService 전용 graph 분석
            DependencyInjectionGraphBuilder.AnalyzeDependencyBundles(dependencyServices, interfaceMap, registeredTypes);

            return services;
        }

        private static void AddKeyed(this IServiceCollection services, ServiceLifetime lifetime, 
            Type serviceType, object key, Type implementationType)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton: 
                    services.AddKeyedSingleton(serviceType, key, implementationType); 
                    break;
                case ServiceLifetime.Scoped: 
                    services.AddKeyedScoped(serviceType, key, implementationType); 
                    break;
                case ServiceLifetime.Transient: 
                    services.AddKeyedTransient(serviceType, key, implementationType); 
                    break;
            }
        }
    }
}