using Microsoft.Extensions.DependencyInjection;
using ModularFramework.Data.Query.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.Data.Query.Extensions
{
    public static class QueryServiceExtensions
    {
        /// <summary>
        /// QueryProvider를 싱글톤 서비스로 등록합니다.
        /// </summary>
        /// <param name="services">서비스 컬렉션</param>
        /// <param name="setupAction">
        /// [선택] 상세 옵션을 설정하기 위한 액션입니다. 
        /// 설정하지 않을 경우 다음 기본값이 적용됩니다:
        /// <list type="bullet">
        /// <item><description><B>BasePath:</B> 실행파일경로/Queries</description></item>
        /// <item><description><B>CheckIntervalMs:</B> 60,000ms (1분)</description></item>
        /// </list>
        /// </param>
        /// <returns>설정된 서비스 컬렉션</returns>
        public static IServiceCollection AddQueryProvider(this IServiceCollection services, Action<QueryProviderOptions> setupAction = null)
        {
            // 1. 사용자가 설정 액션을 전달했다면 적용
            if (setupAction != null)
            {
                var options = new QueryProviderOptions();
                setupAction(options);

                // 입력된 값을 전역 옵션 클래스에 반영
                QueryProviderOption.BasePath = options.BasePath;
                QueryProviderOption.CheckIntervalMs = options.CheckIntervalMs;
            }

            if (!Directory.Exists(QueryProviderOption.BasePath))
            {
                throw new Exception("Query directory not found. Please check QueryProviderOptions.");
            }

            // 2. QueryProvider를 싱글톤으로 등록
            // 생성자 내부에서 EnsureDirectory() 등을 통해 경로 유효성을 스스로 검증합니다.
            services.AddSingleton<QueryProvider>();

            return services;
        }
    }
}