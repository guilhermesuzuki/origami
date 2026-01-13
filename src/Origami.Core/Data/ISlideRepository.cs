using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public interface ISlideRepository
    {
        /// <summary>
        /// Retrieves all slides from the system.
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSystemFile> GetSlides();

        /// <summary>
        /// Gets the current slide from the system.
        /// </summary>
        /// <returns></returns>
        OrigamiSystemFile? GetSlide();
    }
}
