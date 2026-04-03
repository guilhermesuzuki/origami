using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Origami.LocalizationAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TextUsageAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray<DiagnosticDescriptor>.Empty;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel
                .GetSymbolInfo(invocation)
                .Symbol as IMethodSymbol;

            if (symbol == null)
                return;

            // Check class
            if (symbol.ContainingType.Name != "Text")
                return;

            // Check method
            if (new string[] { "Get", "Lower", "Upper", "Original" }.Contains(symbol.Name) == false)
                return;

            // Extract argument
            var arg = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (arg == null)
                return;

            var constant = context.SemanticModel.GetConstantValue(arg.Expression);

            if (!constant.HasValue)
                return;

            var key = constant.Value?.ToString();

            // 🎯 THIS is your detected key
            System.Diagnostics.Debug.WriteLine($"Detected key: {key}");
        }
    }
}
