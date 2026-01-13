using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ICategories<TCategory, TContent>
        where TCategory : OrigamiCategory
        where TContent : BaseContent
    {
        /// <summary>
        /// Returns all categories associated with a post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        IEnumerable<TCategory> GetCategories(TContent post);
    }
}
