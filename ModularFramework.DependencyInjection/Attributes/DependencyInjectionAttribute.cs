using ModularFramework.DependencyInjection.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Attributes
{
    /// <summary>
    /// Attribute 기반 자동 DI 등록을 위한 기본 어트리뷰트입니다.
    /// </summary>
    /// <remarks>
    /// AddAutoRegister 수행 시 어셈블리 스캔 대상으로 인식되며,
    /// 지정된 수명 주기와 인터페이스 매핑 정보를 기반으로
    /// DI 컨테이너에 자동 등록됩니다.
    ///
    /// DependencyServiceAttribute는 본 클래스를 상속하여
    /// Property Injection 기능을 추가로 제공합니다.
    ///
    /// Key 값이 지정된 경우 Keyed Service로 등록됩니다.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DependencyInjectionAttribute : Attribute
    {
        /// <summary>
        /// 지정된 수명 주기와 인터페이스 매핑 정보로
        /// 자동 등록 대상을 선언합니다.
        /// </summary>
        /// <param name="dependencyInjectionLifeTimeType">
        /// 서비스 등록 시 사용할 수명 주기입니다.
        /// </param>
        /// <param name="interfaces">
        /// 구현 클래스와 함께 등록할 인터페이스 목록입니다.
        /// </param>
        public DependencyInjectionAttribute(
            DependencyInjectionLifeTime dependencyInjectionLifeTimeType,
            params Type[] interfaces)
        {
            DependencyInjectionLifeTimeType = dependencyInjectionLifeTimeType;
            InterfaceTypes = interfaces ?? [];
        }

        /// <summary>
        /// 서비스 수명 주기
        /// </summary>
        public DependencyInjectionLifeTime DependencyInjectionLifeTimeType { get; }

        /// <summary>
        /// 함께 등록할 인터페이스 목록
        /// </summary>
        public Type[] InterfaceTypes { get; }

        /// <summary>
        /// Keyed Service 등록용 Key
        /// </summary>
        public string Key { get; init; } = string.Empty;
    }
}