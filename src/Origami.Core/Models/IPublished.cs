namespace Origami.Core.Models
{
    public interface IPublished
    {
        /// <summary>
        /// Is this published or not?
        /// </summary>
        bool IsPublished { get; set; }

        /// <summary>
        /// Date/Time this was published
        /// </summary>
        DateTime? DatePublished { get; set; }
    }
}
