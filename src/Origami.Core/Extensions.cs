using System.Globalization;

namespace Origami.Core
{
    public static class Extensions
    {
        public static bool En(this CultureInfo culture)
        {
            return culture.Name.StartsWith("en", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
