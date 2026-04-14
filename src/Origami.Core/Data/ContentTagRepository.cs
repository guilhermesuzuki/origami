using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public class ContentTagRepository : 
        RepositoryOuterLayer<OrigamiContentTag>,
        IContentTagRepository
    {
        protected readonly IValidator<OrigamiContentTag> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentTagRepository(
            IValidator<OrigamiContentTag> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string DeletePermission => nameof(OrigamiRole.DeleteTags);
        public override string ReadPermission => nameof(OrigamiRole.ViewTags);
        public override string PurgePermission => nameof(OrigamiRole.PurgeTags);
        public override string UpdatePermission => nameof(OrigamiRole.EditTags);

        public Result RefreshCache(Guid blog, string before, string current)
        {
            using var db = DbContextFactory.CreateDbContext();

            var q1 = from b in this.DbContextFactory.ReadFromCache<OrigamiBlog>(MemoryCache)
                     join v in this.DbContextFactory.ReadFromCache<OrigamiContent>(MemoryCache) on b.Id equals v.BlogId
                     join t in this.ReadFromCache() on v.Id equals t.ContentId
                     where b.Id == blog
                     where t.Tag == before
                     select t;

            var q2 = from b in this.DbContextFactory.ReadFromCache<OrigamiBlog>(MemoryCache)
                     join v in this.DbContextFactory.ReadFromCache<OrigamiContent>(MemoryCache) on b.Id equals v.BlogId
                     join t in db.Set<OrigamiContentTag>().AsNoTracking() on v.Id equals t.ContentId
                     where b.Id == blog
                     where t.Tag == current
                     select t;

            q1.ToList().Each(this.PurgeCache);
            q2.ToList().Each(this.CreateCache);

            return new();
        }

        public override Result<OrigamiContentTag> Update(DataOperationContext<OrigamiContentTag> ctx)
        {
            if (ctx.EntityBeforeModifications != null)
            {
                using (var db = DbContextFactory.CreateDbContext())
                {
                    var blog = from a in db.ContentTags.AsNoTracking()
                               join b in db.Contents.AsNoTracking() on a.ContentId equals b.Id
                               where a.NanoId == ctx.Entity.NanoId
                               select b.BlogId;

                    var query = from a in db.ContentTags.AsNoTracking()
                                join b in db.Contents.AsNoTracking() on a.ContentId equals b.Id
                                where b.BlogId == blog.FirstOrDefault()
                                where a.Tag == ctx.EntityBeforeModifications.Tag
                                select a;

                    query.ExecuteUpdate(setters => setters.SetProperty(t => t.Tag, ctx.Entity.Tag));

                    this.RefreshCache(blog.FirstOrDefault() ?? throw new InvalidOperationException("Blog not found"), ctx.EntityBeforeModifications.Tag, ctx.Entity.Tag);
                }
                return new(ctx.Entity);
            }
            return new(ctx.Entity, "Entity before modifications is null, update cannot proceed");
        }
    }
}
