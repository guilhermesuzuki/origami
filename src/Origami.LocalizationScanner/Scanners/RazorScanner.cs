using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.LocalizationScanner.Scanners
{
    public class RazorScanner
    {
        public IEnumerable<ExtractedString> Scan(string filePath)
        {
            var engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create("."),
                b => { });

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var source = RazorSourceDocument.ReadFrom(fileStream, filePath);
            var codeDoc = engine.ProcessDesignTime(
                source,
                null,
                Array.Empty<RazorSourceDocument>(),
                Array.Empty<TagHelperDescriptor>());

            var documentNode = codeDoc.GetDocumentIntermediateNode();

            foreach (var node in documentNode.FindDescendantNodes<HtmlContentIntermediateNode>())
            {
                if (node is HtmlContentIntermediateNode htmlNode)
                {
                    foreach (var child in htmlNode.Children)
                    {
                        if (child is IntermediateToken token && token.IsHtml)
                        {
                            var text = token.Content?.Trim();

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                yield return new ExtractedString
                                {
                                    Text = text,
                                    File = filePath,
                                    Line = 0,
                                    Context = "razor-html"
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
