using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IBaseRepository<T> where T : IId
    {
        Text Text { get; }

        IWebRootPath WebRootPath { get; }

        void PurgeChildrenFromCache(T entity);

        void PurgeRelationshipsFromCache(T entity);

        List<T> ReadFromCache();

        List<X> ReadFromCache<X>() where X : class, new();

        void RefreshCache();
    }
}
