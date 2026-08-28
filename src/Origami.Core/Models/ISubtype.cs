namespace Origami.Core.Models
{
    public interface ISubtype
    {
        /// <summary>
        /// Gets or sets the content subtype or specialization for the model,
        /// such as a special message severity or type, or a special page subtype.
        /// </summary>
        string Subtype { get; set; }
    }
}
