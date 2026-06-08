using Microsoft.Extensions.DependencyInjection;
using ModularFramework.Core.Reflection;
using ModularFramework.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularFramework.DependencyInjection.Runtime
{
    static class DependencyInjectionFactory
    {
        internal static Dictionary<Type, ServiceInjectionPlan> ServiceActivationCache { get; } = [];

        internal static Dictionary<Type, List<Type>> InterfaceImplementations { get; } = [];

        internal static object CreateDependencyService(IServiceProvider sp, Type serviceType)
        {
            if (!ServiceActivationCache.TryGetValue(serviceType, out var plan))
            {
                throw new InvalidOperationException(
                    $"[DependencyInjection] '{serviceType.FullName}'는 Activation Cache가 없습니다."
                );
            }
            // 1. 객체 생성 (parameterless constructor)
            object instance = ActivatorUtilities.CreateInstance(sp, serviceType)
                 ?? throw new InvalidOperationException(
                     $"[DependencyInjection] '{serviceType.FullName}' 생성 실패"
                 );
            // 2. property injection
            foreach (var prop in plan.BundleCache)
            {
                // 이미 값 있으면 skip
                if (prop.Getter(instance) != null)
                    continue;
                object propValue = !string.IsNullOrEmpty(prop.Key)
                    ? sp.GetRequiredKeyedService(prop.ResolveType, prop.Key)
                    : sp.GetRequiredService(prop.ResolveType);
                prop.Setter(instance, propValue);
            }

            return instance;
        }
    }

    class ServiceInjectionPlan(InjectPropertyInfo[] bundleProperties)
    {
        internal InjectPropertyInfo[] BundleCache { get; } = bundleProperties;
    }

    class InjectPropertyInfo
    {
        public InjectPropertyInfo(PropertyInfo property, Type resolveType, string key)
        {
            this.Getter = ReflectionDelegateFactory.CreateGetter(property);
            this.Setter = ReflectionDelegateFactory.CreateSetter(property);
            this.ResolveType = resolveType;
            this.Key = key;
        }

        public Action<object, object> Setter { get; }

        public Func<object, object> Getter { get; }

        /// <summary>
        /// 실제로 컨테이너에서 꺼낼 클래스 타입
        /// </summary>
        public Type ResolveType { get; } //= resolveType;

        public string Key { get; }
    }
}