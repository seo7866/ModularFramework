using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using ModularFramework.DependencyInjection.Analyzers.Rules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ModularFramework.DependencyInjection.Analyzers
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DependencyServiceClassAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DependencyInjectionRules.MDI001);

        public override void Initialize(AnalysisContext context)
        {            
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeClass, SymbolKind.NamedType);
        }

        private static void AnalyzeClass(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;

            // 💡 global::이 붙은 형태와 안 붙은 형태 딱 두 가지만 정확하게 매칭합니다.
            // 이렇게 하면 앞뒤에 엉뚱한 글자가 붙은 유사 어트리뷰트 오탐을 100% 차단합니다.
            string targetAttribute = "ModularFramework.DependencyInjection.Attributes.DependencyServiceAttribute";

            var hasAttribute = type.GetAttributes().Any(a =>
            {
                if (a.AttributeClass == null) return false;

                string fullName = a.AttributeClass.ToDisplayString();
                return fullName == targetAttribute || fullName == $"global::{targetAttribute}";
            });

            if (!hasAttribute)
                return;

            // 명시적 생성자 선언 여부 체크
            if (type.Constructors.Any(c => !c.IsImplicitlyDeclared))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DependencyInjectionRules.MDI001,
                        // 💡 클래스 전체보다는 에러 원인인 생성자 위치에 정확히 줄을 그어주면 더 친절합니다.
                        type.Constructors.First(c => !c.IsImplicitlyDeclared).Locations.FirstOrDefault() ?? type.Locations.FirstOrDefault(),
                        type.Name));
            }
        }
    }
}
