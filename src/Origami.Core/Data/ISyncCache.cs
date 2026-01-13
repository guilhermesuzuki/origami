using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISyncCache<T>
        where T : IId
    {
        /// <summary>
        /// Sync the cache based on an <paramref name="operation"/> and a given <paramref name="id"/>,
        /// hiting the database to pull the latest information (except for purge operation)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        void SyncCache(Guid id, string operation);
    }
}
