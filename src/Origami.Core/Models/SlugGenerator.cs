using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Origami.Core.Models
{
    public static class SlugGenerator
    {
        public static string Generate(string? value, int? maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            // Normalize Unicode so equivalent characters are treated consistently.
            value = value.Normalize(NormalizationForm.FormKC);

            var builder = new StringBuilder(value.Length);
            var previousWasSeparator = false;

            foreach (var character in value)
            {
                var category = char.GetUnicodeCategory(character);

                if (IsLetterOrNumber(category))
                {
                    builder.Append(char.ToLowerInvariant(character));
                    previousWasSeparator = false;
                    continue;
                }

                // Combining marks should be preserved.
                //
                // This is important for languages such as Hindi:
                // नमस्ते
                //
                // Removing marks would corrupt the text.
                if (IsMark(category))
                {
                    builder.Append(character);
                    previousWasSeparator = false;
                    continue;
                }

                // Convert whitespace, punctuation and symbols into a single '-'.
                if (!previousWasSeparator && builder.Length > 0)
                {
                    builder.Append('-');
                    previousWasSeparator = true;
                }
            }

            var slug = builder.ToString().Trim('-');

            if (maxLength is > 0 && slug.Length > maxLength.Value)
            {
                slug = slug[..maxLength.Value].TrimEnd('-');
            }

            return slug;
        }

        private static bool IsLetterOrNumber(UnicodeCategory category)
        {
            return category is
                UnicodeCategory.UppercaseLetter or
                UnicodeCategory.LowercaseLetter or
                UnicodeCategory.TitlecaseLetter or
                UnicodeCategory.ModifierLetter or
                UnicodeCategory.OtherLetter or
                UnicodeCategory.DecimalDigitNumber or
                UnicodeCategory.LetterNumber or
                UnicodeCategory.OtherNumber;
        }

        private static bool IsMark(UnicodeCategory category)
        {
            return category is
                UnicodeCategory.NonSpacingMark or
                UnicodeCategory.SpacingCombiningMark or
                UnicodeCategory.EnclosingMark;
        }
    }
}
