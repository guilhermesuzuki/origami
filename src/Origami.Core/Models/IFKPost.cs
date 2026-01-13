namespace Origami.Core.Models
{
    public interface IFKPost : IPostId
    {
        /// <summary>
        /// Post (FK)
        /// </summary>
        OrigamiPost? Post { get; set; }
    }
}
