using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// Interface for reading entities from cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadCache<T> where T : IId
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        string KeyForCaching { get; }

        /// <summary>
        /// Cache in Memory
        /// </summary>
        IMemoryCache MemoryCache { get; }
    }
}
