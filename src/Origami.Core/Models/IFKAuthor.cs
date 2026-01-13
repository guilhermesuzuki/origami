namespace Origami.Core.Models
{
    public interface IFKAuthor : IAuthorId
    {
        /// <summary>
        /// Author (FK)
        /// </summary>
        OrigamiUser? Author { get; set; }
    }
}
