using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// email class
    /// </summary>
    [Serializable]
    [XmlRoot("email")]
    public class Email : CommonInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("tooltip")]
        public string ToolTip { get; set; } = string.Empty;
    }
}