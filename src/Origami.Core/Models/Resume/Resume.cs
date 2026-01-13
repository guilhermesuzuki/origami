using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// class representing the whole resume
    /// </summary>
    [Serializable]
    [XmlRoot("resume")]
    public class Resume :
        IId,
        ITitle,
        IDescription
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Resume()
          : base()
        {

        }

        /// <summary>
        /// description made
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; } = string.Empty;

        [XmlElement("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// webpages keywords (meta content)
        /// </summary>
        [XmlElement("keywords")]
        public string Keywords { get; set; } = string.Empty;

        /// <summary>
        /// language on the resume file
        /// </summary>
        [XmlAttribute("language")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// personal info
        /// </summary>
        [XmlElement("personal")]
        public PersonalInfo Personal { get; set; } = new();

        /// <summary>
        /// resume subtitle
        /// </summary>
        [XmlElement("sub-title")]
        public string SubTitle { get; set; } = string.Empty;

        /// <summary>
        /// resume title
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// personal info
        /// </summary>
        [XmlElement("work")]
        public WorkInfo Work { get; set; } = new();
    }
}