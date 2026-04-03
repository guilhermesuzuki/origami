using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.LocalizationGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Linq;
    using System.Text;

    [Generator]
    public class LocalizationGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Optional debugger attach
            // System.Diagnostics.Debugger.Launch();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var keys = new HashSet<string>();

            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                var root = tree.GetRoot();

                var invocations = root.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>();

                foreach (var invocation in invocations)
                {
                    var symbol = semanticModel
                        .GetSymbolInfo(invocation)
                        .Symbol as IMethodSymbol;

                    if (symbol == null)
                        continue;

                    // 🔥 Match your Text class
                    if (symbol.ContainingType.Name != "Text")
                        continue;

                    if (symbol.Name is not ("Get" or "Lower" or "Upper" or "Original"))
                        continue;

                    var arg = invocation.ArgumentList.Arguments.FirstOrDefault();
                    if (arg == null)
                        continue;

                    var constant = semanticModel.GetConstantValue(arg.Expression);

                    if (!constant.HasValue)
                        continue;

                    var key = constant.Value?.ToString();

                    if (!string.IsNullOrWhiteSpace(key))
                        keys.Add(key);
                }
            }

            GenerateClass(context, keys);
            GenerateResxLike(context, keys);
        }

        private void GenerateClass(GeneratorExecutionContext context, HashSet<string> keys)
        {
            var sb = new StringBuilder();

            sb.AppendLine("namespace GeneratedLocalization;");
            sb.AppendLine("public static class Keys");
            sb.AppendLine("{");

            foreach (var key in keys.OrderBy(x => x))
            {
                sb.AppendLine($"    public const string {Sanitize(key)} = \"{key}\";");
            }

            sb.AppendLine("}");

            context.AddSource("LocalizationKeys.g.cs", sb.ToString());
        }

        private void GenerateResxLike(GeneratorExecutionContext context, HashSet<string> keys)
        {
            var sb = new StringBuilder();

            sb.AppendLine("/* RESX PREVIEW");
            sb.AppendLine("<root>");

            foreach (var key in keys.OrderBy(x => x))
            {
                sb.AppendLine($@"  <data name=""{key}""><value>{key}</value></data>");
            }

            sb.AppendLine("</root>");
            sb.AppendLine("*/");

            context.AddSource("LocalizationResxPreview.g.cs", sb.ToString());
        }

        private string Sanitize(string key)
        {
            return key.Replace(" ", "_")
                      .Replace("-", "_")
                      .Replace("'", "");
        }
    }
}
