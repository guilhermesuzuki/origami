namespace Origami.Core.Models
{
    public interface IFKVideo : IVideoId
    {
        /// <summary>
        /// Video (FK)
        /// </summary>
        OrigamiVideo? Video { get; set; }
    }
}
