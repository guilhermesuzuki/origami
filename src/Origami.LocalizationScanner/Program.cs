using Origami.LocalizationScanner;
using Origami.LocalizationScanner.Scanners;
using System.Text.Json;

Console.WriteLine("Hello, World!");

var root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

root = "c:\\Projects\\github - origami\\src\\";

var results = new List<ExtractedString>();

var csScanner = new CSharpScanner();
var razorScanner2 = new RazorScanner();

foreach (var file in Directory.GetFiles(root, "*.*", SearchOption.AllDirectories))
{
    if (file.EndsWith(".cs"))
        results.AddRange(csScanner.Scan(file));

    if (file.EndsWith(".razor"))
        results.AddRange(razorScanner2.Scan(file));
}

// Filter + dedupe
var cleaned = results
    .Where(x => Filters.IsUserFacing(x.Text))
    .GroupBy(x => x.Text)
    .Select(g => g.First())
    .ToList();

var json = JsonSerializer.Serialize(cleaned, new JsonSerializerOptions
{
    WriteIndented = true
});

File.WriteAllText("output.json", json);

Console.WriteLine($"Done. Found {cleaned.Count} strings.");

var keys = from item in cleaned
           orderby item.Text
           select item.Text;

await File.WriteAllLinesAsync("keys.txt", keys.Distinct());