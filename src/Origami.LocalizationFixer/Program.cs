using System.Globalization;
using System.Xml.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        FixTheTranslations(new CultureInfo("de-DE"));
        FixTheTranslations(new CultureInfo("es-ES"));
        FixTheTranslations(new CultureInfo("fr-CA"));
        FixTheTranslations(new CultureInfo("fr-FR"));
        FixTheTranslations(new CultureInfo("hi-IN"));
        FixTheTranslations(new CultureInfo("it-IT"));
        FixTheTranslations(new CultureInfo("ja-JP"));
        FixTheTranslations(new CultureInfo("ko-KR"));
        FixTheTranslations(new CultureInfo("pt-PT"));
        FixTheTranslations(new CultureInfo("zh-HANS"));
    }

    private static void FixTheTranslations(CultureInfo culture)
    {
        string textFilePath = $"Input/{culture.Name}.txt";
        string resxFilePath = $"Output/Text.{culture.Name}.resx";

        if (!File.Exists(textFilePath) || !File.Exists(resxFilePath))
        {
            Console.WriteLine("Input files not found.");
            return;
        }

        // Step 1: Load translations into dictionary
        var translations = LoadTranslations(textFilePath);

        // Step 2: Load RESX XML
        var doc = XDocument.Load(resxFilePath);

        // Step 3: Iterate over <data> elements
        var dataElements = doc.Descendants("data");

        int updatedCount = 0;

        foreach (var data in dataElements)
        {
            var nameAttr = data.Attribute("name");
            var valueElement = data.Element("value");

            if (nameAttr == null || valueElement == null)
                continue;

            string key = nameAttr.Value;

            // Step 4: Replace if translation exists
            if (translations.TryGetValue(key, out string? translatedValue) && translatedValue != null)
            {
                valueElement.Value = translatedValue;
                updatedCount++;
            }
        }

        // Step 5: Save updated RESX
        doc.Save(resxFilePath);

        Console.WriteLine($"Done. Updated {updatedCount} entries.");
    }

    private static Dictionary<string, string> LoadTranslations(string filePath)
    {
        var dict = new Dictionary<string, string>();

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains(':'))
            {
                continue;
            }

            var parts = line.Split(':', 2);

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            if (!dict.ContainsKey(key))
            {
                dict[key] = value;
            }
        }

        return dict;
    }
}