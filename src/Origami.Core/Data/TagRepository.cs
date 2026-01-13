using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// Dummy Repository for now
    /// </summary>
    public class TagRepository :
        RepositoryOuterLayer<OrigamiTag>,
        ITagRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public TagRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string DeletePermission => nameof(OrigamiRole.DeleteTags);
        public override string ReadPermission => nameof(OrigamiRole.ViewTags);
        public override string PurgePermission => nameof(OrigamiRole.PurgeTags);
        public override string UpdatePermission => nameof(OrigamiRole.EditTags);

        public override Result<OrigamiTag> Purge(DataOperationContext<OrigamiTag> ctx)
        {
            var hub = new Result<OrigamiTag>();

            using (var dbContext = DbContextFactory.CreateDbContext())
            {
                var rows1 = dbContext.PostTags.Include(x => x.Post)
                    .Where(x => x.Post!.BlogId == ctx.Entity.BlogId)
                    .Where(x => x.Tag == ctx.Entity.Name)
                    .ExecuteDelete();

                var rows2 = dbContext.VideoTags.Include(x => x.Video)
                    .Where(x => x.Video!.BlogId == ctx.Entity.BlogId)
                    .Where(x => x.Tag == ctx.Entity.Name)
                    .ExecuteDelete();

                hub.RowsAffected += rows1;
                hub.RowsAffected += rows2;
            }

            return hub;
        }

        public override Result<OrigamiTag> Update(DataOperationContext<OrigamiTag> ctx)
        {
            if (ctx.EntityBeforeModifications != null)
            {
                using (var dbContext = DbContextFactory.CreateDbContext())
                {
                    dbContext.PostTags.Include(x => x.Post)
                        .Where(x => x.Post!.BlogId == ctx.Entity.BlogId)
                        .Where(x => x.Tag == ctx.EntityBeforeModifications.Name)
                        .ExecuteUpdate(setters => setters.SetProperty(t => t.Tag, ctx.Entity.Name));

                    dbContext.VideoTags.Include(x => x.Video)
                        .Where(x => x.Video!.BlogId == ctx.Entity.BlogId)
                        .Where(x => x.Tag == ctx.EntityBeforeModifications.Name)
                        .ExecuteUpdate(setters => setters.SetProperty(t => t.Tag, ctx.Entity.Name));

                    dbContext.SaveChanges();
                }
                return new(ctx.Entity);
            }
            return new(ctx.Entity, "Entity before modifications is null, update cannot proceed");
        }
    }
}
