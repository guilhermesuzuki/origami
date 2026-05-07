using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IMyMemoryCache : 
        IId,
        IMemoryCache
    {
        /// <summary>
        /// Cache keys
        /// </summary>
        IEnumerable<object> Keys { get; }

        /// <summary>
        /// Clears the entire cache
        /// </summary>
        void Clear();

        /// <summary>
        /// Reads from cache, hitting the database if necessary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> Read<T>() where T : class;
    }
}
