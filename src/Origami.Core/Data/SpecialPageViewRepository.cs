using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class SpecialPageViewRepository :
        RepositoryOuterLayer<OrigamiSpecialPageView>,
        ISpecialPageViewRepository
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SpecialPageViewRepository(
            IConfiguration configuration,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _configuration = configuration;
        }

        public Task Update(IEnumerable<SpecialPageViewTotal> entities)
        {
            entities.Each(entity => this.SetViews(new(entity.SpecialPageId), entity.TotalViews));
            return Task.CompletedTask;
        }

        public async Task<List<SpecialPageViewTotal>> FastRead()
        {
            using var db = DbContextFactory.CreateDbContext();
            var sql = "SELECT COUNT_BIG(pv.Id) as TotalViews, p.Id as SpecialPageId FROM dbo.oi_SpecialPageViews pv RIGHT JOIN dbo.oi_SpecialPages p ON p.Id = pv.SpecialPageId GROUP BY p.Id";
            return await db.Database.SqlQueryRaw<SpecialPageViewTotal>(sql).ToListAsync();
        }

        /// <summary>
        /// Returns the total number of views from a page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public long GetViews(OrigamiSpecialPage page)
        {
            return this.Views(page);
        }

        public void SetViews(OrigamiSpecialPage entity, long count) => this.Views(entity, count);
    }
}
