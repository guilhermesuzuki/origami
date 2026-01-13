using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [Serializable, XmlRoot("certifications")]
    public class Certifications : CommonInfo
    {
        public Certifications() : base()
        {
            EachCertification = new List<Certification>();
        }

        /// <summary>
        /// information
        /// </summary>
        [XmlElement("info")]
        public string Information { get; set; } = string.Empty;

        /// <summary>
        /// list of certificates
        /// </summary>
        [XmlElement("certification", Type = typeof(Certification))]
        public List<Certification> EachCertification { get; set; } = new();
    }
}