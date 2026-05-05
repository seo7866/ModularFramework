using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using ModularFramework.Configuration.Attributes;
using ModularFramework.Core.Resolvers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularFramework.Configuration.Extensions
{
    public static class ConfigurationServiceExtensions
    {
        /// <summary>
        /// 어셈블리를 스캔하여 <see cref="ConfigurationRegisterAttribute"/>가 적용된 클래스를 찾고, 
        /// 해당 클래스를 구성(Configuration) 섹션과 바인딩하여 DI 컨테이너에 등록합니다.
        /// </summary>
        /// <param name="services">DI 컨테이너 인스턴스</param>
        /// <param name="configuration">애플리케이션 설정 인스턴스</param>
        /// <returns>구성이 완료된 <see cref="IServiceCollection"/></returns>
        /// <exception cref="InvalidOperationException">파일 로드가 필요하지만 <paramref name="configuration"/>이 IConfigurationBuilder를 구현하지 않을 경우 발생할 수 있습니다.</exception>
        public static IServiceCollection AddAutoConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. 이 라이브러리를 참조하는 어셈블리들을 싹 긁어옵니다.
            var assemblies = AssemblyDependencyResolver.GetDependentAssemblies(Assembly.GetExecutingAssembly());
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ConfigurationRegisterAttribute>();
                if (attr == null) 
                    continue;

                // 2. 파일 이름이 지정되어 있고, 아직 로드되지 않았다면 추가 (IConfigurationRoot 필요)
                if (!string.IsNullOrWhiteSpace(attr.FileName) && configuration is IConfigurationBuilder builder)
                {
                    // 중복 로드 방지 체크
                    var alreadyLoaded = builder.Sources.OfType<JsonConfigurationSource>()
                                               .Any(s => string.Equals(s.Path, attr.FileName, StringComparison.OrdinalIgnoreCase));

                    if (!alreadyLoaded)
                    {
                        builder.AddJsonFile(attr.FileName, optional: false, reloadOnChange: true);
                    }
                }

                // 3. 네임스페이스를 포함한 FullName(없으면 Name)을 섹션 키로 사용하여 설정을 찾습니다.
                // 예: "YourNamespace.ConfigClassName": { ... } 구조를 탐색합니다.
                var section = configuration.GetSection(type.FullName ?? type.Name);

                // 4. 객체 생성 및 바인딩
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    section.Bind(instance);

                    // 5. 싱글톤으로 등록하여 어디서든 주입받아 사용 가능하게 함
                    services.AddSingleton(type, instance);
                }
            }

            return services;
        }
    }
}
