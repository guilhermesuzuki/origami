using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer1Validation<T> :
        RepositoryLayer0Data<T>,
        ICreateValidation<T>,
        IUpdateValidation<T>,
        IDeleteValidation<T>,
        IPurgeValidation<T>
        where T : class, IId, new()
    {
        protected RepositoryLayer1Validation(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual Result<T> CreateValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual Result<T> DeleteValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual Result<T> PurgeValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual Result<T> UpdateValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        #region Validation methods

        public virtual bool IsCycleDetected(DataOperationContext<T> ctx, IList<T> list)
        {
            if (ctx.Entity is IParentIdNull<T> parent)
            {
                var entity = ctx.Entity;

                if (list.Id(entity.Id) != null) return true;
                if (entity.Id == parent.ParentId) return true;

                list.Add(entity);

                if (parent.ParentId != null)
                {
                    var db = ReadFromDatabase().Id(parent.ParentId.GetValueOrDefault());
                    if (db != null)
                    {
                        return this.IsCycleDetected(new(ctx.User, db), list);
                    }
                }
            }
            return false;
        }

        public virtual Result<T> ValidateSlug(DataOperationContext<T> ctx)
        {
            var validation = new Result<T>(ctx.Entity);
            if (ctx.Entity is ISlug slug && slug.Slug.Has() == true)
            {
                IEnumerable<T> db = this.ReadFromDatabase().ToList();
                if (ctx.Entity is IBlogId blogId)
                {
                    db = db.OfType<IBlogId>().Where(x => x.BlogId == blogId.BlogId).OfType<T>();
                }
                db = db.OfType<ISlug>().Where(x => x.Slug == slug.Slug).OfType<T>();
                db = db.Where(x => x.Id != ctx.Entity.Id);
                if (db.Any() == true)
                {
                    validation.Error = Text.Original("Slug is already in use");
                }
            }
            return validation;
        }

        #endregion
    }
}
