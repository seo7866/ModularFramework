using ModularFramework.DependencyInjection.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Attributes
{
    /// <summary>
    /// 특정 클래스의 의존성 주입(Dependency Injection) 설정을 선언적으로 정의하는 특성(Attribute)입니다.
    /// </summary>
    /// <remarks>
    /// 이 어트리뷰트가 부여된 클래스는 <see cref="AddAutoRegister"/> 메서드에 의해 어셈블리 스캔 시 자동으로 DI 컨테이너에 등록됩니다.
    /// </remarks>
    /// <param name="dependencyInjectionLifeTimeType">서비스의 수명 주기 (예: Singleton, Scoped 등)</param>
    /// <param name="interfaces">컨테이너를 통해 접근할 인터페이스 타입들 (가변 인자)</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DependencyInjectionAttribute(DependencyInjectionLifeTime dependencyInjectionLifeTimeType, params Type[] interfaces) : Attribute
    {
        /// <summary>
        /// 주입할 서비스의 수명 주기를 가져옵니다.
        /// </summary>
        public DependencyInjectionLifeTime DependencyInjectionLifeTimeType { get; internal set; } = dependencyInjectionLifeTimeType;

        /// <summary>
        /// 서비스 클래스가 구현하며, 컨테이너에 함께 등록될 인터페이스 타입 목록을 가져옵니다.
        /// </summary>
        public Type[] InterfaceTypes { get; } = interfaces;
    }
}