using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("education")]
    public class Education : CommonInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Education() : base() { }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("college")]
        public List<College> Colleges { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("trade-school")]
        public List<TradeSchool> TradeSchools { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("high-school")]
        public HighSchool HighSchool_ { get; set; } = new();

        /// <summary>
        /// abstract class for any school course
        /// </summary>
        [Serializable, XmlRoot("course")]
        public class Course
        {
            /// <summary>
            /// School short name
            /// </summary>
            [XmlAttribute("short-name")]
            public string ShortName { get; set; } = string.Empty;

            /// <summary>
            /// year when this course was finished
            /// </summary>
            [XmlAttribute("finished")]
            public string Finished { get; set; } = string.Empty;

            /// <summary>
            /// Finished Description
            /// </summary>
            [XmlAttribute("finished-description")]
            public string FinishedDescription { get; set; } = string.Empty;

            /// <summary>
            /// duration amount of time for the course
            /// </summary>
            [XmlAttribute("duration")]
            public float Duration { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [XmlAttribute("duration-type")]
            public eDurationType DurationType { get; set; }

            /// <summary>
            /// Duration Description
            /// </summary>
            [XmlAttribute("duration-description")]
            public string DurationDescription { get; set; } = string.Empty;

            /// <summary>
            /// school name
            /// </summary>
            [XmlElement("school")]
            public string School { get; set; } = string.Empty;

            /// <summary>
            /// course name or title
            /// </summary>
            [XmlElement("course-name")]
            public string CourseName { get; set; } = string.Empty;

            /// <summary>
            /// basic description
            /// </summary>
            [XmlElement("description")]
            public string Description { get; set; } = string.Empty;

            /// <summary>
            /// type of school course
            /// </summary>
            [XmlAttribute("description")]
            public string Type { get; set; } = string.Empty;

            /// <summary>
            /// duration type for courses
            /// </summary>
            public enum eDurationType : byte
            {
                [XmlEnum(Name = "y")]
                Years = 0,
                [XmlEnum(Name = "m")]
                Months = 1,
                [XmlEnum(Name = "h")]
                Hours = 2,
            }

            /// <summary>
            /// if the course has a file associated to it or if there's a certificate to it
            /// </summary>
            [XmlElement("file")]
            public string File { get; set; } = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        [XmlRoot("college")]
        public class College : Course
        {
            /// <summary>
            /// default constructor
            /// </summary>
            public College() : base() { }

            [XmlArray("files"), XmlArrayItem("certification")]
            public List<Certification> Certificates { get; set; } = new();
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        [XmlRoot("trade-school")]
        public class TradeSchool : Course
        {
            /// <summary>
            /// default constructor
            /// </summary>
            public TradeSchool() : base() { }

            [XmlArray("files"), XmlArrayItem("certification")]
            public List<Certification> Certificates { get; set; } = new();
        }

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        [XmlRoot("high-school")]
        public class HighSchool : Course
        {
            /// <summary>
            /// 
            /// </summary>
            public HighSchool() : base() { }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("extra-courses")]
    public class ExtraCourses : CommonInfo
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public ExtraCourses() : base() { }

        /// <summary>
        /// languages
        /// </summary>
        [XmlElement("course")]
        public List<Education.Course> Courses { get; set; } = new();
    }
}