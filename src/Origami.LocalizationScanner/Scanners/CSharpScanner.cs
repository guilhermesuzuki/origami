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
    }
}
