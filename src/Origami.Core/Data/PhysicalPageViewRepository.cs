using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PhysicalPageViewRepository :
        RepositoryOuterLayer<OrigamiPhysicalPageView>,
        IPhysicalPageViewRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PhysicalPageViewRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override Result<OrigamiPhysicalPageView> CanCreate(DataOperationContext<OrigamiPhysicalPageView> ctx)
        {
            return new(ctx.Entity);
        }

        public async Task<List<PhysicalPageViewTotal>> FastRead()
        {
            using var dbContextFactory = DbContextFactory.CreateDbContext();
            var sql = "SELECT COUNT_BIG(pv.Id) as TotalViews, p.Id as PhysicalPageId FROM dbo.oi_PhysicalPageViews pv RIGHT JOIN dbo.oi_PhysicalPages p ON p.Id = pv.PhysicalPageId GROUP BY p.Id";
            return await dbContextFactory.Database.SqlQueryRaw<PhysicalPageViewTotal>(sql).ToListAsync();
        }

        /// <summary>
        /// Returns the total number of views from a page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public long GetViews(OrigamiPhysicalPage page)
        {
            return this.Views(page);
        }

        public long GetViews<T>(T entity) where T : IId
        {
            var x = 0L;
            var key = entity.KeyForCachingViews();
            var options = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) };

            // already in cache, return it
            if (this.MemoryCache.TryGetValue(key, out x))
            {
                return x;
            }

            try
            {
                using var db = DbContextFactory.CreateDbContext();

                var query = from view in db.Set<OrigamiPhysicalPageView>()
                            where view.ContentId != null
                            where view.ContentId == entity.Id
                            select view;

                x = query.LongCount();
                return x;
            }
            finally
            {
                this.MemoryCache.Set(key, x, options);
            }
        }

        public void SetViews(OrigamiPhysicalPage entity, long count) => this.Views(entity, count);

        public Task Update(IEnumerable<PhysicalPageViewTotal> entities)
        {
            entities.Each(entity => this.SetViews(new(entity.PhysicalPageId), entity.TotalViews));
            return Task.CompletedTask;
        }
    }
}
