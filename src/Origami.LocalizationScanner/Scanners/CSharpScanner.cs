namespace Origami.LocalizationScanner.Scanners;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class CSharpScanner
{
    public IEnumerable<ExtractedString> Scan(string filePath)
    {
        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();

        /*
        var literals = root.DescendantNodes()
            .OfType<LiteralExpressionSyntax>()
            .Where(x => x.IsKind(SyntaxKind.StringLiteralExpression));

        foreach (var literal in literals)
        {
            yield return new ExtractedString
            {
                Text = literal.Token.ValueText,
                File = filePath,
                Line = literal.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                Context = "csharp"
            };
        }
        */

        var allowedMethods = new[] { "Get", "Lower", "Upper", "Original" };

        var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

        foreach (var invocation in invocations)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax m && allowedMethods.Contains(m.Name.Identifier.Text) && m.Expression is IdentifierNameSyntax id && id.Identifier.Text == "Text")
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
                        Context = $"C# method: {m.Name.Identifier.Text}"
                    };
                }
            }
        }
    }
}
