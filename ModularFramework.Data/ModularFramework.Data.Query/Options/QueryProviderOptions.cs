using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.Data.Query.Options
{
    /// <summary>
    /// AddQueryProvider 메서드에서만 사용하는 임시 옵션 설정 객체입니다.
    /// </summary>
    public class QueryProviderOptions
    {
        /// <summary>
        /// SQL 파일들이 위치한 기본 경로를 설정합니다. (기본값: ./Queries)
        /// </summary>
        public string BasePath { get; set; } = QueryProviderOption.BasePath;

        /// <summary>
        /// 파일 변경 감지 주기(밀리초)를 설정합니다. (기본값: 60,000ms)
        /// </summary>
        public uint CheckIntervalMs { get; set; } = QueryProviderOption.CheckIntervalMs;
    }
}