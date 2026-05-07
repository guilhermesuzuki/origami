using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IBaseRepository<T> :
        IReadFromCache<T>
        where T : IId
    {
        Text Text { get; }

        IWebRootPath WebRootPath { get; }

        void PurgeChildrenFromCache(T entity);

        void PurgeRelationshipsFromCache(T entity);

        void RefreshCache();
    }
}
