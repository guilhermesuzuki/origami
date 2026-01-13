using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [XmlRoot("facebook")]
    public class Facebook
    {
        [XmlAttribute("username")]
        public string Username { get; set; } = string.Empty;

        [XmlText]
        public string Link { get; set; } = string.Empty;
    }
}
