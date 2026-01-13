using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [Serializable]
    [XmlRoot("certification")]
    public class Certification : CommonInfo
    {
        /// <summary>
        /// short name for this certification
        /// </summary>
        [XmlAttribute("strong")]
        public byte Strong { get; set; }

        /// <summary>
        /// short name for this certification
        /// </summary>
        [XmlAttribute("short-name")]
        public string Short { get; set; } = string.Empty;

        /// <summary>
        /// name of the certification
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// url for the certificate
        /// </summary>
        [XmlElement("file")]
        public string File { get; set; } = string.Empty;
    }
}