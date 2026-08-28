namespace Origami.LocalizationScanner
{
    public static class Filters
    {
        public static bool IsUserFacing(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (text.All(char.IsDigit)) return false;
            if (text.Contains("://")) return false;
            if (!text.Any(char.IsLetter)) return false;
            if (text.StartsWith("data:image")) return false;
            return true;
        }
    }
}
