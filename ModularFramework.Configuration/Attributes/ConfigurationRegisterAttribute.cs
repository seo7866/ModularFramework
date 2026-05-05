using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.Configuration.Attributes
{
    /// <summary>
    /// 설정(Configuration) 클래스를 자동으로 스캔하고 바인딩하기 위한 특성입니다.
    /// </summary>
    /// <remarks>
    /// 이 어트리뷰트가 부여된 클래스는 <see cref="AddAutoConfiguration"/> 메서드를 통해 
    /// 특정 JSON 파일의 섹션과 자동으로 바인딩되어 DI 컨테이너에 싱글톤으로 등록됩니다.
    /// </remarks>
    /// <param name="fileName">해당 설정 데이터가 포함된 JSON 파일명 (예: "appsettings.json" 또는 "custom.json")</param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationRegisterAttribute(string fileName = "") : Attribute
    {
        /// <summary>
        /// 설정 값을 읽어올 대상 파일 경로를 가져옵니다.
        /// 값이 비어있을 경우 "YourNamespace.ConfigClassName": { ... } 구조를 탐색합니다.
        /// </summary>
        public string FileName { get; } = fileName;
    }
}
