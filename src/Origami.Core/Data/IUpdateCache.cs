using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IUpdateCache<T> where T : IId
    {
        /// <summary>
        /// Updates the <paramref name="entity"/> in Cache
        /// </summary>
        /// <param name="entity"></param>
        void UpdateCache(T entity);
    }
}
