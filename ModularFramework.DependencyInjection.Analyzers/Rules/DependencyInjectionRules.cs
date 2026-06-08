using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Analyzers.Rules
{
    internal static class DependencyInjectionRules
    {
        public static DiagnosticDescriptor MDI001 { get; } = new DiagnosticDescriptor(
            //id: RuleFactory.RuleIds.MDI001,
            id: "MDI001",
            title: "DependencyService는 생성자를 가질 수 없습니다",
            messageFormat: "{0}은 생성자를 정의할 수 없습니다",
            category: RuleFactory.Prefix,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
