namespace Origami.LocalizationScanner;

public class ExtractedString
{
    public string Text { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public int Line { get; set; }
    public string Context { get; set; } = string.Empty;
}
