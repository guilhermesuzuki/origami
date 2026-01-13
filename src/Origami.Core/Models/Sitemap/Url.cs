using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Origami.Core.Models.Sitemap
{
    [XmlRoot("url")]
    public class Url
    {
        /// <summary>
        /// URL of the page. This URL must begin with the protocol (such as http) and end with a trailing slash, if your web server requires it. This value must be less than 2,048 characters.
        /// </summary>
        [XmlElement("loc")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string loc { get; set; } = string.Empty;

        /// <summary>
        /// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0.
        /// </summary>
        [XmlElement("priority")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public float? priority { get; set; }

        /// <summary>
        /// always, hourly, daily, weekly, monthly, yearly, and never
        /// </summary>
        [XmlElement("changefreq")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string? changefreq { get; set; }

        /// <summary>
        /// Localized versions of this Url
        /// </summary>
        public List<Link> LocalizedVersions { get; set; } = new();
    }
}
