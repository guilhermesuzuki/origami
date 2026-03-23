using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryOuterLayer<T> : RepositoryLayer4Search<T>
        where T : class, IId
    {
        protected RepositoryOuterLayer(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual void RefreshCache()
        {
            var k = KeyForCaching;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Refreshing cache for {k}");
            lock (OrigamiConstants.SyncRoot)
            {
                using var db = DbContextFactory.CreateDbContext();
                var l = db.Set<T>().AsNoTracking().ToList();
                MemoryCache.Set(k, l);
            }
        }
    }
}
