using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PageViewRepository :
        RepositoryOuterLayer<OrigamiPageView>,
        IPageViewRepository
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PageViewRepository(
            IConfiguration configuration,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _configuration = configuration;
        }

        public Task Update(IEnumerable<PageViewTotal> entities)
        {
            entities.Each(entity => this.SetViews(new(entity.PageId), entity.TotalViews));
            return Task.CompletedTask;
        }

        public async Task<List<PageViewTotal>> FastRead()
        {
            using var db = DbContextFactory.CreateDbContext();
            var sql = "SELECT COUNT_BIG(pv.Id) as TotalViews, p.Id as PageId FROM dbo.oi_PageViews pv RIGHT JOIN dbo.oi_Pages p ON p.Id = pv.PageId GROUP BY p.Id";
            return await db.Database.SqlQueryRaw<PageViewTotal>(sql).ToListAsync();
        }

        /// <summary>
        /// Returns the total number of views from a page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public long GetViews(OrigamiPage page)
        {
            return this.Views(page);
        }

        public void SetViews(OrigamiPage entity, long count) => this.Views(entity, count);
    }
}
