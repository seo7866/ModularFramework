using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Analyzers.Rules
{
    internal static class RuleFactory
    {
        internal const string Prefix = "MDI";

        internal static class RuleIds
        {
            //constructor 명시적 생성 금지 (클래스 구조 규칙)
            public const string MDI001 = Prefix + "001";
        }
    }
}
