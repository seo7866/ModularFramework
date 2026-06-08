using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Attributes
{
    /// <summary>
    /// Property 또는 Constructor Parameter에 대한 명시적 DI 주입 설정을 지정합니다.
    /// </summary>
    /// <remarks>
    /// 해당 Attribute는 다음 용도로 사용됩니다:
    /// - 인터페이스 타입의 경우 여러 구현체 중 특정 구현체를 선택
    /// - 자동 DI 매핑이 모호한 경우 명시적 ResolveType 지정
    ///
    /// ⚠ 주의:
    /// - 반드시 구체 클래스(Concrete Type)만 지정할 수 있습니다.
    /// - 인터페이스나 abstract class는 허용되지 않습니다.
    /// - DI 분석 단계에서 검증됩니다.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class DependencyInjectAttribute : Attribute
    {
        /// <summary>
        /// 실제 DI 컨테이너에서 resolve할 구체 타입
        /// </summary>
        internal Type ResolveType { get; init; }

        /// <summary>
        /// 특정 구현체를 명시적으로 지정하여 DI 매핑을 결정합니다.
        /// </summary>
        /// <param name="type">
        /// 반드시 concrete class (non-abstract, non-interface)여야 합니다.
        /// </param>
        /// <exception cref="ArgumentException">
        /// type이 인터페이스이거나 abstract class인 경우 발생합니다.
        /// </exception>
        public DependencyInjectAttribute(Type type)
        {
            // 추상 클래스가 아니고 인터페이스가 아닌 '클래스'만 허용
            if (!type.IsClass || type.IsAbstract)
            {
                throw new ArgumentException($"[DependencyInject] {type.Name}은(는) 구체적인 클래스(Concrete Class)여야 합니다.");
            }
            ResolveType = type;
        }
    }
}