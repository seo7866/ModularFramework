using ModularFramework.DependencyInjection.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Attributes
{
    /// <summary>
    /// DI 컨테이너에 의해 관리되는 서비스 클래스를 표시하는 Attribute입니다.
    /// </summary>
    /// <remarks>
    /// 이 Attribute가 적용된 클래스는 다음 규칙을 따릅니다:
    /// - 생성자는 parameterless만 허용됩니다.
    /// - 모든 의존성 주입은 Property Injection 방식으로 처리됩니다.
    /// - DI 컨테이너가 런타임에 인스턴스를 생성하고 관리합니다.
    /// </remarks>
    /// <param name="dependencyInjectionLifeTimeType">
    /// 해당 서비스의 라이프사이클 (Singleton / Scoped / Transient)
    /// </param>
    /// <param name="interfaces">
    /// 외부에 노출할 인터페이스 타입 목록
    /// (DI 등록 시 Service → Interface 매핑에 사용됩니다)
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DependencyServiceAttribute(DependencyInjectionLifeTime dependencyInjectionLifeTimeType, params Type[] interfaces) 
        : DependencyInjectionAttribute(dependencyInjectionLifeTimeType, interfaces)
    {
        
    }
}