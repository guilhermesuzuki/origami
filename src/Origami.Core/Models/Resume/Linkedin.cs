using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [XmlRoot("linkedin")]
    public class Linkedin
    {
        [XmlAttribute("username")]
        public string Username { get; set; } = string.Empty;

        [XmlText]
        public string Link { get; set; } = string.Empty;
    }
}
