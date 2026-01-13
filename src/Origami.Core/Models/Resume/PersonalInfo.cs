using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [Serializable]
    [XmlRoot("personal")]
    public class PersonalInfo : CommonInfo
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public PersonalInfo()
        {
            Emails = new List<Email>();
            Education = new Education();
            ExtraCourses = new ExtraCourses();
            Skills = new Skills();
            Telephones = new List<Telephone>();
        }

        /// <summary>
        /// First name
        /// </summary>
        [XmlAttribute("firstname")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name
        /// </summary>
        [XmlAttribute("lastname")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// full name
        /// </summary>
        [XmlIgnore]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        /// <summary>
        /// Nationality
        /// </summary>
        [XmlElement("location")]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Nationality
        /// </summary>
        [XmlElement("nationality")]
        public string Nationality { get; set; } = string.Empty;

        /// <summary>
        /// birthday
        /// </summary>
        [XmlElement("birthday")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// facebook address
        /// </summary>
        [XmlElement("facebook")]
        public Facebook Facebook { get; set; } = new();

        /// <summary>
        /// twitter address
        /// </summary>
        [XmlElement("twitter")]
        public Twitter Twitter { get; set; } = new();

        /// <summary>
        /// list of available e-mails
        /// </summary>
        [XmlArray("emails")]
        [XmlArrayItem("email")]
        public List<Email> Emails { get; set; } = new();

        /// <summary>
        /// skype id
        /// </summary>
        [XmlElement("skype")]
        public string SkypeId { get; set; } = string.Empty;

        /// <summary>
        /// education information
        /// </summary>
        [XmlElement("education")]
        public Education Education { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("extra-courses")]
        public ExtraCourses ExtraCourses { get; set; } = new();

        /// <summary>
        /// skills
        /// </summary>
        [XmlElement("skills")]
        public Skills Skills { get; set; } = new();

        /// <summary>
        /// List of Telephones
        /// </summary>
        [XmlArray("telephones"), XmlArrayItem("telephone")]
        public List<Telephone> Telephones { get; set; } = new();

        [XmlElement("languages")]
        public Languages Languages { get; set; } = new();
    }
}