using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ICreateCache<T> where T : IId
    {
        /// <summary>
        /// Adds the <paramref name="entity"/> in Cache
        /// </summary>
        /// <param name="entity"></param>
        void CreateCache(T entity);
    }
}
