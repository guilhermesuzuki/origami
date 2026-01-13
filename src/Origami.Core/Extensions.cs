using System.Globalization;

namespace Origami.Core
{
    public static class Extensions
    {
        public static bool En(this CultureInfo culture)
        {
            return culture.Name.StartsWith("en");
        }
        public static bool Es(this CultureInfo culture)
        {
            return culture.Name.StartsWith("es");
        }
        public static bool Ja(this CultureInfo culture)
        {
            return culture.Name.StartsWith("ja");
        }
        public static bool Pt(this CultureInfo culture)
        {
            return culture.Name.StartsWith("pt");
        }
    }
}
