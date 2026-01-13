using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// professional experience
    /// </summary>
    [Serializable]
    [XmlRoot("work")]
    public class WorkInfo
    {
        public WorkInfo()
        {
            Experience = new Experience();
            Certifications = new Certifications();
        }

        /// <summary>
        /// linkedin profile link
        /// </summary>
        [XmlElement("linkedin")]
        public Linkedin LinkedIn { get; set; } = new();

        /// <summary>
        /// certifications
        /// </summary>
        [XmlElement("certifications")]
        public Certifications Certifications { get; set; }

        /// <summary>
        /// professional experience
        /// </summary>
        [XmlElement("experience")]
        public Experience Experience { get; set; }
    }
}