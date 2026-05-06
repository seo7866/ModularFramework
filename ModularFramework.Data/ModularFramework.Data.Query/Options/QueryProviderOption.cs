using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.Data.Query.Options
{
    /// <summary>
    /// QueryProvider의 동작을 제어하는 전역 설정 클래스입니다.
    /// 프로그램 시작 시점(Main 등)에서 설정하며, 실행 중 경로가 변경되면 모든 캐시가 자동으로 갱신됩니다.
    /// </summary>
    public static class QueryProviderOption
    {
        private static string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queries");
        /// <summary>
        /// SQL 파일들을 찾을 기본 루트 경로입니다.
        /// 기본값은 애플리케이션 실행 디렉터리 하위의 'Queries' 폴더입니다.
        /// (예: C:\MyApp\Queries)
        /// 변경 시 <see cref="OnBasePathChanged"/> 이벤트를 통해 활성화된 모든 QueryProvider의 캐시를 즉시 초기화합니다.
        /// </summary>
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
        /// <summary>
        /// 파일의 수정 시간을 물리적으로 확인할 시간 간격(밀리초)입니다.
        /// 이 주기 동안은 디스크 IO 없이 메모리에 캐싱된 쿼리 문자열을 반환하여 성능을 최적화합니다.
        /// </summary>
        public static uint CheckIntervalMs
        {
            get => _checkIntervalMs;
            set => _checkIntervalMs = value;
        }

        /// <summary>
        /// 경로 변경 시 QueryProvider들이 감지할 수 있는 이벤트
        /// </summary>
        internal static event EventHandler OnBasePathChanged;
    }
}