using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ModularFramework.Core.Helpers
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// 현재 도메인의 모든 DLL을 스캔하여 특정 어셈블리를 참조하고 있는 의존 어셈블리 목록을 가져옵니다.
        /// </summary>
        /// <param name="targetAssembly">기준이 되는 어셈블리 (기본값: 호출한 어셈블리)</param>
        public static Assembly[] GetDependentAssemblies(Assembly targetAssembly = null)
        {
            // 1. 기준 어셈블리 설정
            targetAssembly ??= Assembly.GetCallingAssembly();
            string targetFullName = targetAssembly.FullName!;

            // 2. 이미 로드된 어셈블리 가져오기 (동적 어셈블리 제외)
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToList();
            var loadedLocations = loadedAssemblies.Select(a => a.Location).ToList();

            // 3. 실행 폴더 내의 모든 DLL을 강제로 로드하여 누락된 참조 확인
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var dllPath in Directory.GetFiles(baseDir, "*.dll"))
            {
                if (loadedLocations.Contains(dllPath)) 
                    continue;
                try
                {
                    // 로드되지 않은 DLL이 있다면 도메인에 올림
                    var loaded = Assembly.LoadFrom(dllPath);
                    loadedAssemblies.Add(loaded);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Assembly Load Failed] {dllPath}: {ex.Message}");
                }
            }

            // 4. 기준 어셈블리를 참조하고 있는 것들만 필터링하여 반환
            return loadedAssemblies
                .Where(a => a.Equals(targetAssembly) ||
                            a.GetReferencedAssemblies().Any(r => r.FullName == targetFullName))
                .ToArray();
        }
    }
}
