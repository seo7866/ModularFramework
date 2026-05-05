using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.Data.Query
{
    /// <summary>
    /// QueryProvider의 전역 옵션입니다.
    /// 프로그램 시작 시점에 이 값을 수정하면 전체 QueryProvider에 반영됩니다.
    /// </summary>
    public static class QueryProviderOption
    {
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queries");
        public static string BasePath
        {
            get => _basePath;
            set
            {
                if (_basePath != value)
                {
                    _basePath = value;
                    // 경로가 바뀌면 기존 캐시를 모두 날리도록 이벤트를 발생시킴
                    OnBasePathChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        private static uint _checkIntervalMs = 60000;
        public static uint CheckIntervalMs
        {
            get => _checkIntervalMs;
            set => _checkIntervalMs = value;
        }

        /// <summary>
        /// 경로 변경 시 QueryProvider들이 감지할 수 있는 이벤트
        /// </summary>
        internal static event EventHandler? OnBasePathChanged;
    }
}
