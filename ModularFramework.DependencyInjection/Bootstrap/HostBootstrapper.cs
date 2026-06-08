using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularFramework.DependencyInjection.Enums;
using ModularFramework.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Bootstrap
{
    public static class HostBootstrapper
    {
        /// <summary>
        /// Auto DI 등록 + Host 생성 + 즉시 시작까지 수행하는 통합 빌더
        /// </summary>
        /// <param name="configure">추가적인 수동 서비스 등록 (AddSingleton 등)</param>
        /// <param name="overrides">AutoRegister 단계에서 특정 타입의 라이프사이클을 강제로 덮어쓰기 위한 설정</param>
        /// <returns>시작까지 완료된 IHost 인스턴스 (이미 실행 상태)</returns>
        public static IHost BuildAutoHost(Action<IServiceCollection> configure = null, Dictionary<Type, DependencyInjectionLifeTime> overrides = null)
        {
            // Host는 Build 순간 DI container(ServiceProvider)를 고정(immmutable)한다.
            // 즉, Build 이후에는 IServiceCollection 변경이 불가능하다.
            IHost host = Host.CreateDefaultBuilder()
                // DI 구성 단계
                .ConfigureServices((context, s) =>
                {
                    // 1. 사용자 정의 추가 등록
                    configure?.Invoke(s);
                    // 2. Auto scanning 기반 DI 등록
                    s.AddAutoRegister(overrides);
                })
                // Host 생성 (이 시점에서 DI graph 확정)
                .Build();
            // Build 이후 Host는 실행 상태로 전환된다.
            // 내부적으로 ServiceProvider가 고정되며 이후 등록 변경 불가
            host.Start();
            return host;
        }
    }
}