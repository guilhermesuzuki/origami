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
            ImmutableArray.Create(MissingKeyRule);

        private static readonly DiagnosticDescriptor MissingKeyRule =
            new DiagnosticDescriptor(
                id: "LOC001",
                title: "Localization key detected",
                messageFormat: "Key used: '{0}'",
                category: "Localization",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel
                .GetSymbolInfo(invocation)
                .Symbol as IMethodSymbol;

            var methods = new string[] { "Get", "Lower", "Upper", "Original" };

            if (symbol == null)
                return;

            if (symbol.ContainingType.Name != "Text")
                return;

            if (!methods.Contains(symbol.Name))
                return;

            var arg = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (arg == null)
                return;

            var constant = context.SemanticModel.GetConstantValue(arg.Expression);

            if (!constant.HasValue)
                return;

            var key = constant.Value?.ToString();

            var diagnostic = Diagnostic.Create(
                MissingKeyRule,
                arg.GetLocation(),
                key);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
