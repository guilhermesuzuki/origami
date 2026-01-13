namespace Origami.Core.Models
{
    public interface IDraft
    {
        /// <summary>
        /// Is this content still a draft?
        /// </summary>
        bool? IsDraft { get; set; }
    }
}
