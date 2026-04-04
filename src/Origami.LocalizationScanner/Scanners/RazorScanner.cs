using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Origami.LocalizationScanner.Scanners
{
    public class RazorScanner
    {
        public IEnumerable<ExtractedString> Scan(string filePath)
        {
            var code = string.Join(Environment.NewLine, File.ReadAllLines(filePath));

            var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, RazorProjectFileSystem.Create("."), builder => { });

            var sourceDoc = RazorSourceDocument.Create(code, "test.razor");

            var codeDoc = projectEngine.Process(sourceDoc, null, Array.Empty<RazorSourceDocument>(), Array.Empty<TagHelperDescriptor>());

            var generatedCSharp = codeDoc.GetCSharpDocument().GeneratedCode;

            // Now use Roslyn on generatedCSharp
            var tree = CSharpSyntaxTree.ParseText(generatedCSharp);
            var root = tree.GetRoot();

            var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

            var compilation = CSharpCompilation.Create("Analysis").AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)).AddSyntaxTrees(tree);

            var model = compilation.GetSemanticModel(tree);

            var allowedProperties = new[] { "Text", "_text", "text" };
            var allowedMethods = new[] { "Get", "Lower", "Upper", "Original" };

            foreach (var invocation in invocations)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax m && allowedMethods.Contains(m.Name.Identifier.Text) && m.Expression is IdentifierNameSyntax id && allowedProperties.Contains(id.Identifier.Text))
                {
                    //Console.WriteLine($"Found: {invocation}");
                    foreach (var argument in invocation.ArgumentList.Arguments)
                    {
                        var key = argument.ToString().Trim('\"');
                        yield return new ExtractedString
                        {
                            Text = key,
                            File = filePath,
                            Line = invocation.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                            Context = $"Razor method: {m.Name.Identifier.Text}"
                        };
                    }
                }
            }
        }
    }
}
