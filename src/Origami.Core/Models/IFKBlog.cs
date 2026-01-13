namespace Origami.Core.Models
{
    public interface IFKBlog : IBlogId
    {
        /// <summary>
        /// Blog (FK)
        /// </summary>
        OrigamiBlog? Blog { get; set; }
    }
}
