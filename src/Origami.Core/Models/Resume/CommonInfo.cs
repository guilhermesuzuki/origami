using System.Xml.Serialization;

namespace Origami.Core.Models.Resume
{
    /// <summary>
    /// abstract information class, containing common properties between models
    /// </summary>
    public abstract class CommonInfo
    {
        /// <summary>
        /// default constructor
        /// </summary>
        protected CommonInfo()
        {
            Description = string.Empty;
        }

        /// <summary>
        /// [atribute] description
        /// </summary>
        [XmlAttribute("description")]
        public virtual string Description { get; set; }
    }
}