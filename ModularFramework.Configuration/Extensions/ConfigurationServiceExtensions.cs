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
        /// [Web용] 어셈블리 스캔을 통한 구성(Configuration) 자동 등록
        /// ASP.NET Core의 WebApplicationBuilder.Configuration과 같이 
        /// IConfiguration과 IConfigurationBuilder를 동시에 구현하는 객체에 적합합니다.
        /// </summary>
        public static IServiceCollection AddAutoConfigurationForWeb(this IServiceCollection services, IConfiguration configuration)
        {
            // 내부적으로 동일한 로직을 호출하되, builder 자리에 configuration을 캐스팅해서 전달
            return services.AddAutoConfigurationInternal(configuration, configuration as IConfigurationBuilder);
        }

        /// <summary>
        /// [Desktop/Console용] 어셈블리 스캔을 통한 구성(Configuration) 자동 등록
        /// 빌드된 설정 객체와 설정을 확장할 수 있는 빌더 객체를 각각 전달받습니다.
        /// </summary>
        public static IServiceCollection AddAutoConfigurationForApp(this IServiceCollection services, IConfiguration configuration, IConfigurationBuilder builder)
        {
            // 명시적으로 받은 builder를 사용하여 파일을 추가하고 다시 빌드합니다.
            return services.AddAutoConfigurationInternal(configuration, builder);
        }

        private static IServiceCollection AddAutoConfigurationInternal(this IServiceCollection services, IConfiguration configuration, IConfigurationBuilder builder)
        {
            var assemblies = AssemblyDependencyResolver.GetDependentAssemblies(Assembly.GetExecutingAssembly());
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ConfigurationRegisterAttribute>();
                if (attr == null)
                    continue;

                IConfiguration currentSource = configuration;

                // 1. 파일 추가 및 데이터 갱신 (Builder가 있을 때만 실행)
                if (!string.IsNullOrWhiteSpace(attr.FileName) && builder != null)
                {
                    var alreadyLoaded = builder.Sources.OfType<JsonConfigurationSource>()
                                               .Any(s => string.Equals(s.Path, attr.FileName, StringComparison.OrdinalIgnoreCase));

                    if (!alreadyLoaded)
                    {
                        builder.AddJsonFile(attr.FileName, optional: false, reloadOnChange: true);
                        currentSource = builder.Build(); // 최신 데이터로 갱신
                    }
                }

                // 2. 바인딩 소스 결정 (역순 탐색: FullName -> Name -> Root)
                IConfiguration bindSource;
                var fullSection = currentSource.GetSection(type.FullName ?? string.Empty);
                var nameSection = currentSource.GetSection(type.Name ?? string.Empty);

                if (fullSection.Exists()) 
                    bindSource = fullSection;
                else if (nameSection.Exists()) 
                    bindSource = nameSection;
                else 
                    bindSource = currentSource;

                // 3. 인스턴스 생성 및 등록
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    bindSource.Bind(instance);
                    services.AddSingleton(type, instance);
                }
            }
            return services;
        }
    }
}