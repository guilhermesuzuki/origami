using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoViewRepository :
        RepositoryOuterLayer<OrigamiVideoView>,
        IVideoViewRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoViewRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override Result<OrigamiVideoView> CanCreate(DataOperationContext<OrigamiVideoView> ctx)
        {
            return new(ctx.Entity);
        }

        public override void CreateCache(OrigamiVideoView entity)
        {

        }

        public async Task<List<VideoViewTotal>> FastRead()
        {
            using var db = DbContextFactory.CreateDbContext();
            var sql = "SELECT COUNT_BIG(vv.Id) as TotalViews, v.Id as VideoId FROM dbo.oi_VideoViews vv RIGHT JOIN dbo.oi_Videos v ON v.Id = vv.VideoId GROUP BY v.Id;";
            return await db.Database.SqlQueryRaw<VideoViewTotal>(sql).ToListAsync();
        }

        public long GetViews(OrigamiVideo video)
        {
            return this.Views(video);
        }

        public void SetViews(OrigamiVideo entity, long count) => this.Views(entity, count);

        public Task Update(IEnumerable<VideoViewTotal> entities)
        {
            entities.Each(entity =>
            {
                entity.Video = new() { Id = entity.VideoId };
                this.SetViews(entity.Video!, entity.TotalViews);
            });

            return Task.CompletedTask;
        }
    }
}
