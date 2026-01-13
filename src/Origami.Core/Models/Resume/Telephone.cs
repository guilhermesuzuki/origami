using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// telephone class
    /// </summary>
    [Serializable]
    [XmlRoot("telephone")]
    public class Telephone : CommonInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Number { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("tooltip")]
        public string ToolTip { get; set; } = string.Empty;

        /// <summary>
        /// local area code
        /// </summary>
        [XmlAttribute("areaCode")]
        public string AreaCode { get; set; } = string.Empty;

        /// <summary>
        /// country code
        /// </summary>
        [XmlAttribute("countryCode")]
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>
        /// order (in a list)
        /// </summary>
        [XmlAttribute("order")]
        public short Order { get; set; }

        /// <summary>
        /// is this telephone whatsapp?
        /// </summary>
        [XmlAttribute("whatsapp")]
        public bool Whatsapp { get; set; }

        /// <summary>
        /// returns the complete number
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //returning the formatted number
            return string.Format("{0} " + (AreaCode.Has() ? "({1}) " : null) + " {2}", CountryCode, AreaCode, Number).Trim();
        }
    }
}