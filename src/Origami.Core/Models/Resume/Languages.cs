using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [Serializable, XmlRoot("languages")]
    public class Languages : CommonInfo
    {
        [XmlElement("language")]
        public List<Language> EachLanguage { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [Serializable, XmlRoot("language")]
        public class Language
        {
            public Language() : base()
            {
                Courses = new List<Education.Course>();
                Certificates = new List<Certification>();
            }

            /// <summary>
            /// language short name
            /// </summary>
            [XmlAttribute("short-name")]
            public string ShortName { get; set; } = string.Empty;

            /// <summary>
            /// language level
            /// </summary>
            [XmlAttribute("level")]
            public string Level { get; set; } = string.Empty;

            /// <summary>
            /// Courses associated with this language
            /// </summary>
            [XmlElement("course")]
            public List<Education.Course> Courses { get; set; }

            /// <summary>
            /// Certifications/certificates associated with this language
            /// </summary>
            [XmlElement("certification")]
            public List<Certification> Certificates { get; set; }
        }
    }
}