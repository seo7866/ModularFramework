using ModularFramework.Data.Query.Attributes;
using ModularFramework.Data.Query.Interfaces;
using ModularFramework.Data.Query.Options;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ModularFramework.Data.Query
{
    public class QueryProvider : IQueryExecutor
    {
        private readonly ConcurrentDictionary<MethodBase, QueryCacheEntry> _methodCache = new();

        public QueryProvider()
        {
            // 옵션의 경로 변경 이벤트 구독
            QueryProviderOption.OnBasePathChanged += (s, e) => ClearCache();
            EnsureDirectory();
        }

        /// <summary>
        /// 메서드 정보를 기반으로 SQL 쿼리를 가져옵니다.
        /// </summary>
        public string GetQuery(MethodBase method)
        {
            method = this.GetActualMethod(method);

            // 1. 메서드별 캐시 항목 가져오기 (없으면 생성)
            var entry = _methodCache.GetOrAdd(method, m => new QueryCacheEntry(GetFullPath(m)));

            // 2. 실시간 수정 반영 로직 (수정 시간 체크 후 내용 업데이트)
            return entry.GetOrUpdateContent();
        }

        private void EnsureDirectory()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower().Contains("devenv")) 
                return;
            if (!Directory.Exists(QueryProviderOption.BasePath))
                Directory.CreateDirectory(QueryProviderOption.BasePath);
        }

        private void ClearCache()
        {
            // 1. 기존 캐시 싹 비우기
            _methodCache.Clear();
            // 2. 새 경로 폴더가 있는지 확인
            EnsureDirectory();
            System.Diagnostics.Debug.WriteLine($"[QueryProvider] 경로 변경으로 인해 캐시가 초기화되었습니다: {QueryProviderOption.BasePath}");
        }

        private MethodBase GetActualMethod(MethodBase method)
        {
            // 1. 현재 메서드가 컴파일러가 생성한 비동기 상태 머신(MoveNext)인지 확인
            if (method.Name == "MoveNext" && method.DeclaringType != null && method.DeclaringType.IsDefined(typeof(CompilerGeneratedAttribute), true))
            {
                // 2. 상태 머신 클래스의 '상위 클래스'에서 원본 메서드를 찾음
                var actualType = method.DeclaringType.DeclaringType;
                if (actualType == null) return method;

                // 3. 원본 클래스의 메서드들 중, 이 상태 머신을 사용하는 메서드를 역추적
                var candidate = actualType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .FirstOrDefault(m => m.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType == method.DeclaringType);

                return candidate ?? method;
            }

            return method;
        }

        private string GetFullPath(MethodBase method)
        {
            var attr = method.GetCustomAttribute<QueryPathAttribute>();
            // 어트리뷰트에서 이미 \ -> / 변환이 완료됨
            string relativePath = attr != null && !string.IsNullOrWhiteSpace(attr.Path) ? attr.Path : BuildDefaultPath(method);
            return Path.GetFullPath(Path.Combine(QueryProviderOption.BasePath, relativePath));
        }

        private string BuildDefaultPath(MethodBase method)
        {
            var type = method.DeclaringType ?? throw new InvalidOperationException();
            var assemblyName = type.Assembly.GetName().Name ?? "";

            string nsPath = type.Namespace?.Replace(assemblyName, "").Replace('.', '/') ?? "";
            return $"{nsPath.Trim('/')}/{type.Name}/{method.Name}.sql".TrimStart('/');
        }








        /// <summary>
        /// 메서드당 하나씩 할당되어 파일의 상태를 추적하는 내부 클래스
        /// </summary>
        private class QueryCacheEntry(string filePath)
        {
            private string _content = string.Empty;
            private DateTime _lastFileWriteTime = DateTime.MinValue;
            private long _lastCheckTicks = 0; // 마지막으로 파일을 확인한 시점 (성능을 위해 Ticks 사용)
            private readonly object _lock = new();

            public string GetOrUpdateContent()
            {
                if (!File.Exists(filePath)) 
                    return string.Empty;

                long nowTicks = DateTime.UtcNow.Ticks;
                long lastCheckTicks = Interlocked.Read(ref _lastCheckTicks);

                // 1. 마지막 체크 후 설정된 간격(기본 1분)이 지났는지 확인 (Lock 없이 빠르게)
                if (nowTicks - lastCheckTicks < TimeSpan.FromMilliseconds(QueryProviderOption.CheckIntervalMs).Ticks)
                {
                    return _content; // 간격이 안 지났으면 바로 기존 캐시 반환
                }

                // 2. 간격이 지났다면 Lock 진입하여 갱신 확인
                lock (_lock)
                {
                    // Double-check: 대기하는 동안 다른 스레드가 갱신했는지 확인
                    if (DateTime.UtcNow.Ticks - _lastCheckTicks < TimeSpan.FromMilliseconds(QueryProviderOption.CheckIntervalMs).Ticks)
                    {
                        return _content;
                    }

                    // 3. 실제 파일의 수정 시간 확인
                    var currentFileWriteTime = File.GetLastWriteTimeUtc(filePath);

                    if (currentFileWriteTime > _lastFileWriteTime)
                    {
                        // 파일 내용 갱신
                        _content = CleanQuery(SafeReadFile(filePath));
                        _lastFileWriteTime = currentFileWriteTime;
                    }

                    // 4. 파일 내용 변경 여부와 상관없이 '마지막 체크 시간'은 현재로 갱신
                    Interlocked.Exchange(ref _lastCheckTicks, DateTime.UtcNow.Ticks);
                }

                return _content;
            }

            private string SafeReadFile(string path)
            {
                for (int i = 0; i < 3; i++)
                {
                    try { return File.ReadAllText(path); }
                    catch { Thread.Sleep(100); }
                }
                return string.Empty;
            }

            private string CleanQuery(string sql)
            {
                if (string.IsNullOrWhiteSpace(sql)) return string.Empty;
                return Regex.Replace(sql, @"\s+", " ").Trim();
            }
        }
    }
}
