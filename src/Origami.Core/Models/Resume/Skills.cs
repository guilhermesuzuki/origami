using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("skills")]
    public class Skills : CommonInfo
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Skills() : base() { }

        /// <summary>
        /// text for skills
        /// </summary>
        [XmlText]
        public string Text { get; set; } = string.Empty;
    }
}