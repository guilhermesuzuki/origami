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
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {

        }

        public override Result<OrigamiPhysicalPageView> CanCreate(DataOperationContext<OrigamiPhysicalPageView> ctx)
        {
            return new(ctx.Entity);
        }

        public override void CreateCache(OrigamiPhysicalPageView entity)
        {
            var content = this.MemoryCache.Read<OrigamiContent>().Id(entity.ContentId);
            if (content != null)
            {
                lock (OrigamiConstants.SyncRoot)
                {
                    var key = content.KeyForCachingViews();
                    var count = this.MemoryCache.TryGetValue(key, out long x) ? x : 0;
                    this.MemoryCache.Set(key, count + 1);
                }
            }
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
            var key = entity.KeyForCachingViews();
            return this.MemoryCache.TryGetValue(key, out long x) ? x : 0;
        }

        public void SetViews(OrigamiPhysicalPage entity, long count) => this.Views(entity, count);

        public Task Update(IEnumerable<PhysicalPageViewTotal> entities)
        {
            entities.Each(entity => this.SetViews(new(entity.PhysicalPageId), entity.TotalViews));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Does nothing, views shouldn't be updated in cache
        /// </summary>
        /// <param name="entity"></param>
        public override void UpdateCache(OrigamiPhysicalPageView entity)
        {
            return;
        }

        public override void RefreshCache()
        {
            using var db = DbContextFactory.CreateDbContext();
            var query = from view in db.Set<OrigamiPhysicalPageView>().AsNoTracking() group view by view.ContentId into g select new { ContentId = g.Key, TotalViews = g.LongCount() };

            foreach (var view in query)
            {
                var content = this.MemoryCache.Read<OrigamiContent>().Id(view.ContentId);
                if (content != null)
                {
                    var key = content.KeyForCachingViews();
                    this.MemoryCache.Set(key, view.TotalViews);
                }
            }

            var query2 = from view in db.PhysicalPageViews.AsNoTracking()
                         join page in db.PhysicalPages.AsNoTracking() on view.PhysicalPageId equals page.Id
                         group view by page.Path into g
                         select new { Path = g.Key, TotalViews = g.LongCount() };

            foreach (var view in query2)
            {
                this.MemoryCache.Set(view.Path, view.TotalViews);
            }
        }
    }
}
