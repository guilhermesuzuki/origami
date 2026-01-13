using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Origami.Core.Models.Sitemap
{
    [XmlRoot("link")]
    public class Link
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string rel { get; set; } = "alternate";

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string hreflang { get; set; } = string.Empty;

        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string href { get; set; } = string.Empty;
    }
}
