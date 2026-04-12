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

        var allowedProperties = new[] { "Text", "_text", "text" };
        var allowedMethods = new[] { "Get", "Lower", "Upper", "Original" };

        var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

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
                        Context = $"C# method: {m.Name.Identifier.Text}"
                    };
                }
            }
        }
    }
}
