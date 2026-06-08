using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.Data.Query.Attributes
{
    /// <summary>
    /// 메서드와 매핑될 SQL 파일의 상대 경로를 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class QueryPathAttribute(string path) : Attribute
    {
        // 운영체제 간 호환성을 위해 모든 경로 구분자를 '/'로 통일하여 저장
        public string Path { get; } = path?.Replace('\\', '/') ?? string.Empty;
    }
}