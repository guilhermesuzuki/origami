using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoTagRepository :
        RepositoryOuterLayer<OrigamiVideoTag>,
        IVideoTagRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoTagRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string DeletePermission => nameof(OrigamiRole.DeleteTags);
        public override string ReadPermission => nameof(OrigamiRole.ViewTags);
        public override string UpdatePermission => nameof(OrigamiRole.EditTags);

        public IEnumerable<OrigamiVideoTag> Tags(OrigamiVideo Video)
        {
            return from x in ReadFromCache() where x.VideoId == Video.Id select x;
        }

        public IEnumerable<OrigamiVideo> Videos(OrigamiTag tag)
        {
            return from x in ReadFromCache()
                   where x.Video!.BlogId == tag.BlogId
                   where x.Tag.Like(tag.Name)
                   select x.Video;
        }

        public Result RefreshCache(Guid blog, string before, string current)
        {
            using var db = DbContextFactory.CreateDbContext();

            var q1 = from b in this.ReadFromCache<OrigamiBlog>()
                     join v in this.ReadFromCache<OrigamiVideo>() on b.Id equals v.BlogId
                     join t in this.ReadFromCache() on v.Id equals t.VideoId
                     where b.Id == blog
                     where t.Tag == before
                     select t;

            var q2 = from b in this.ReadFromCache<OrigamiBlog>()
                     join v in this.ReadFromCache<OrigamiVideo>() on b.Id equals v.BlogId
                     join t in db.Set<OrigamiVideoTag>().AsNoTracking() on v.Id equals t.VideoId
                     where b.Id == blog
                     where t.Tag == current
                     select t;

            q1.ToList().Each(this.PurgeCache);
            q2.ToList().Each(this.CreateCache);

            return new();
        }
    }
}
