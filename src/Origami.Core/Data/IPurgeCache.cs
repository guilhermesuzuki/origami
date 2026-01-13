using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPurgeCache<T> where T : IId
    {
        /// <summary>
        /// Removes the <paramref name="entity"/> from Cache
        /// </summary>
        /// <param name="entity"></param>
        void PurgeCache(T entity);
    }
}
