using System.Globalization;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        FixTheTranslations(new CultureInfo("de-DE"));
    }

    static void FixTheTranslations(CultureInfo culture)
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

    static Dictionary<string, string> LoadTranslations(string filePath)
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