using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PostViewRepository :
        RepositoryOuterLayer<OrigamiPostView>,
        IPostViewRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostViewRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override Result<OrigamiPostView> CanCreate(DataOperationContext<OrigamiPostView> ctx)
        {
            return new(ctx.Entity);
        }

        public override void CreateCache(OrigamiPostView entity)
        {

        }

        public async Task<List<PostViewTotal>> FastRead()
        {
            using var dbContextFactory = DbContextFactory.CreateDbContext();
            var sql = "SELECT COUNT_BIG(pv.Id) as TotalViews, p.Id as PostId FROM dbo.oi_PostViews pv RIGHT JOIN dbo.oi_Posts p ON p.Id = pv.PostId GROUP BY p.Id";
            return await dbContextFactory.Database.SqlQueryRaw<PostViewTotal>(sql).ToListAsync();
        }

        public long GetViews(OrigamiPost post)
        {
            return this.Views(post);
        }

        public void SetViews(OrigamiPost entity, long count) => this.Views(entity, count);

        public Task Update(IEnumerable<PostViewTotal> entities)
        {
            entities.Each(entity =>
            {
                entity.Post = new() { Id = entity.PostId };
                this.SetViews(entity.Post!, entity.TotalViews);
            });
            return Task.CompletedTask;
        }
    }
}
