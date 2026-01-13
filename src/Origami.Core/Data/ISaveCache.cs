using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISaveCache<T> where T : IId
    {
        /// <summary>
        /// Adds or updates the <paramref name="entity"/> in Cache
        /// </summary>
        /// <param name="entity"></param>
        void SaveCache(T entity);

        /// <summary>
        /// Adds or updates <paramref name="entities"/> in cache
        /// </summary>
        /// <param name="entities"></param>
        void SaveCache(IEnumerable<T> entities);
    }
}
