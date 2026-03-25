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
        protected readonly IContentTagRepository _contentTagRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public TagRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IContentTagRepository contentTagRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _contentTagRepository = contentTagRepository;
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
                var rows1 = from t in dbContext.ContentTags.AsNoTracking()
                            join c in dbContext.Contents.AsNoTracking() on t.ContentId equals c.Id
                            where c.BlogId == ctx.Entity.BlogId
                            where t.Tag == ctx.Entity.Tag
                            select t;

                hub.RowsAffected += rows1.ExecuteDelete();
            }

            return hub;
        }

        public override Result<OrigamiTag> Update(DataOperationContext<OrigamiTag> ctx)
        {
            if (ctx.EntityBeforeModifications != null)
            {
                using (var dbContext = DbContextFactory.CreateDbContext())
                {
                    var query = from t in dbContext.ContentTags.AsNoTracking()
                             join c in dbContext.Contents.AsNoTracking() on t.ContentId equals c.Id
                             where c.BlogId == ctx.Entity.BlogId
                             where t.Tag == ctx.EntityBeforeModifications.Tag
                             select t;

                    query.ExecuteUpdate(setters => setters.SetProperty(t => t.Tag, ctx.Entity.Tag));
                    dbContext.SaveChanges();
                }
                return new(ctx.Entity);
            }
            return new(ctx.Entity, "Entity before modifications is null, update cannot proceed");
        }

        public override void UpdateCache(OrigamiTag entity)
        {
            base.UpdateCache(entity);

            var before = this.ReadFromCache().Id(entity.Id);
            if (before != null)
            {
                _contentTagRepository.RefreshCache(entity.BlogId.GetValueOrDefault(), before.Tag, entity.Tag);
                return;
            }

            throw new InvalidOperationException("Entity not found in cache, cache update cannot proceed");
        }
    }
}
