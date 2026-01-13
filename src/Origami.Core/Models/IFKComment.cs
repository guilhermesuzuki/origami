namespace Origami.Core.Models
{
    public interface IFKComment<T> where T : BaseComment
    {
        /// <summary>
        /// Comment Id (FK)
        /// </summary>
        Guid CommentId { get; set; }

        /// <summary>
        /// Comment (FK)
        /// </summary>
        T? Comment { get; set; }
    }
}
