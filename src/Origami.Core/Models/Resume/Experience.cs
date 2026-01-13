using System.Xml;
using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// professional experience
    /// </summary>
    [Serializable]
    [XmlRoot("experience")]
    public class Experience : CommonInfo
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Experience() : base()
        {
            Jobs = new List<Job>();
        }

        /// <summary>
        /// jobs experience
        /// </summary>
        [XmlElement("job")]
        public List<Job> Jobs { get; set; }

        /// <summary>
        /// task class
        /// </summary>
        [Serializable]
        [XmlRoot("task")]
        public class Task
        {
            /// <summary>
            /// Title
            /// </summary>
            [XmlAttribute("title")]
            public string Title { get; set; } = string.Empty;

            /// <summary>
            /// Task description
            /// </summary>
            [XmlIgnore]
            public string Description { get; set; } = string.Empty;

            [XmlAnyElement("description")]
            public XmlElement DescriptionElement
            {
                get
                {
                    var x = new XmlDocument();
                    x.LoadXml(string.Format("<Text>{0}</Text>", Description));
                    return x.DocumentElement!;
                }
                set { Description = value.InnerXml; }
            }

            /// <summary>
            /// Company worked for (in case of a project)
            /// </summary>
            [XmlAttribute("company-worked-for")]
            public string CompanyWorkedFor { get; set; } = string.Empty;

            /// <summary>
            /// Company's website
            /// </summary>
            [XmlAttribute("website")]
            public string Website { get; set; } = string.Empty;
        }

        /// <summary>
        /// job class
        /// </summary>
        [Serializable]
        [XmlRoot("job")]
        public class Job
        {
            public Job()
            {
                Tasks = new List<Task>();
            }

            /// <summary>
            /// Header message
            /// </summary>
            [XmlIgnore]
            public string Header { get; set; } = string.Empty;

            /// <summary>
            /// 
            /// </summary>
            [XmlAnyElement("header")]
            public XmlElement HeaderHTML
            {
                get
                {
                    var x = new XmlDocument();
                    x.LoadXml(string.Format("<Text>{0}</Text>", Header));
                    return x.DocumentElement!;
                }
                set { Header = value.InnerXml; }
            }

            [XmlElement("address")]
            public List<Address> Addresses { get; set; } = new List<Address>();

            /// <summary>
            /// List of Google Maps URLs
            /// </summary>
            [XmlElement("google.maps")]
            public List<string> GoogleMaps { get; set; } = new List<string>();

            /// <summary>
            /// job function related
            /// </summary>
            [XmlAttribute("function")]
            public string Function { get; set; } = string.Empty;

            /// <summary>
            /// company related to the job
            /// </summary>
            [XmlAttribute("company")]
            public string Company { get; set; } = string.Empty;

            /// <summary>
            /// location of the job
            /// </summary>
            [XmlAttribute("location"), Obsolete]
            public string Location { get; set; } = string.Empty;

            /// <summary>
            /// started on
            /// </summary>
            [XmlAttribute("started")]
            public DateTime Started { get; set; }

            /// <summary>
            /// finished on
            /// </summary>
            [XmlAttribute("finished")]
            public DateTime Finished { get; set; }

            /// <summary>
            /// lists of tasks of this job
            /// </summary>
            [XmlArray("tasks"), XmlArrayItem("task")]
            public List<Task> Tasks { get; set; } = new();

            /// <summary>
            /// company related to the job
            /// </summary>
            [XmlAttribute("company-website")]
            public string Website { get; set; } = string.Empty;

            /// <summary>
            /// 
            /// </summary>
            [XmlIgnore]
            public string Technologies { get; set; } = string.Empty;

            /// <summary>
            /// 
            /// </summary>
            [XmlAnyElement("technologies")]
            public XmlElement DescriptionElement
            {
                get
                {
                    var x = new XmlDocument();
                    x.LoadXml(string.Format("<Text>{0}</Text>", Technologies));
                    return x.DocumentElement!;
                }
                set { Technologies = value.InnerXml; }
            }

            /// <summary>
            /// 
            /// </summary>
            [XmlAttribute("self-employed")]
            public bool SelfEmployed { get; set; }

            [XmlElement("logo")]
            public string Logo { get; set; } = string.Empty;
        }

        [XmlAttribute("self-employed-description")]
        public string SelfEmployedDescription { get; set; } = string.Empty;
    }
}