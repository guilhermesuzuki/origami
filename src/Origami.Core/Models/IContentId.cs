namespace Origami.Core.Models
{
    public interface IContentId
    {
        /// <summary>
        /// Content Id for Pages, Posts, Comments, etc.
        /// </summary>
        Guid ContentId { get; set; }
    }
}
