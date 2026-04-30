using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryBaseLayer<T> :
        ICache<T>
        where T : class, IId
    {
        protected RepositoryBaseLayer(Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory, IMyMemoryCache memoryCache, IWebRootPath webRootPath) : base()
        {
            DbContextFactory = dbContextFactory;
            MemoryCache = memoryCache;
            Text = text;
            WebRootPath = webRootPath;
        }

        public IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }
        public string KeyForCaching => typeof(T).KeyForCaching();
        public IMyMemoryCache MemoryCache { get; }
        public Text Text { get; }
        public IWebRootPath WebRootPath { get; }

        public virtual void CreateCache(T entity)
        {
            var clone = entity.Clone();
            lock (OrigamiConstants.SyncRoot)
            {
                var value = MemoryCache.GetList<T>(KeyForCaching);
                if (value != null)
                {
                    value.RemoveAll(x => x.Id == entity.Id);
                    value.Add(clone);
                }
                else
                {
                    value = [clone];
                }
                MemoryCache.Set(KeyForCaching, value);
            }
        }

        public virtual void PurgeCache(T entity)
        {
            lock (OrigamiConstants.SyncRoot)
            {
                var value = MemoryCache.GetList<T>(KeyForCaching);
                if (value != null)
                {
                    value.RemoveAll(x => x.Id == entity.Id);
                    MemoryCache.Set(KeyForCaching, value);
                }
            }
        }

        public virtual void UpdateCache(T entity)
        {
            var clone = entity.Clone();
            lock (OrigamiConstants.SyncRoot)
            {
                var value = MemoryCache.GetList<T>(KeyForCaching);
                if (value != null)
                {
                    value.RemoveAll(x => x.Id == entity.Id);
                    value.Add(clone);
                }
                else value = [clone];
                MemoryCache.Set(KeyForCaching, value);
            }
        }
    }
}
