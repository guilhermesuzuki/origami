using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// Information/details about an image file
    /// </summary>
    [XmlRoot("additionalInfo"), Serializable]
    public class ImageInfo : AdditionalInfo
    {
        /// <summary>
        /// simple constructor
        /// </summary>
        public ImageInfo() : base() { }

        /// <summary>
        /// location where the image was taken
        /// </summary>
        [Serializable]
        public class Location
        {
            /// <summary>
            /// simple constructor
            /// </summary>
            public Location() : base()
            {

            }

            /// <summary>
            /// culture the text was written on
            /// </summary>
            [XmlAttribute("culture")]
            public string Culture { get; set; } = string.Empty;

            /// <summary>
            /// location text, giving details of the image
            /// </summary>
            [XmlText]
            public string Text { get; set; } = string.Empty;
        }

        /// <summary>
        /// Location information differs between cultures offered in a multi-localized website
        /// </summary>
        [XmlElement("location")]
        public Location[] LocationsByCulture { get; set; } = Array.Empty<Location>();
    }
}