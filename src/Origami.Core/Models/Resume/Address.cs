using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    [Serializable]
    [XmlRoot("address")]
    public class Address
    {
        [Obsolete]
        [XmlAttribute("zip-code")]
        public string ZipCode { get; set; } = string.Empty;

        [XmlAttribute("postal-code")]
        public string PostalCode { get; set; } = string.Empty;

        [XmlAttribute("unit")]
        public string Unit { get; set; } = string.Empty;

        [XmlAttribute("street-number")]
        public string StreetNumber { get; set; } = string.Empty;

        [XmlAttribute("street-name")]
        public string StreetName { get; set; } = string.Empty;

        [XmlAttribute("city")]
        public string City { get; set; } = string.Empty;

        [XmlAttribute("state")]
        public string State { get; set; } = string.Empty;

        [XmlAttribute("country")]
        public string Country { get; set; } = string.Empty;
    }
}