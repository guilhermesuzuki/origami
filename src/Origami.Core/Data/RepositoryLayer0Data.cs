using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer0Data<T> : RepositoryBaseLayer<T>,
        ICrud<T>
        where T : class, IId
    {
        protected RepositoryLayer0Data(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual Result<T> Create(DataOperationContext<T> ctx)
        {
            try
            {
                var clone = ctx.Entity.Clone();

                using (var dbContext = DbContextFactory.CreateDbContext())
                {
                    clone = dbContext.Add(clone).Entity;
                    dbContext.SaveChanges();
                }

                ctx.Entity.Version(clone);

                return new(ctx.Entity);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
        }

        public virtual Result<T> Delete(DataOperationContext<T> ctx)
        {
            try
            {
                var clone = ctx.Entity.Clone();
                if (clone is IDeleted deleted)
                {
                    deleted.IsDeleted = true;
                    using (var db = DbContextFactory.CreateDbContext())
                    {
                        clone = db.Update(clone).Entity;
                        db.SaveChanges();
                    }

                    ctx.Entity.Deleted(clone);
                    ctx.Entity.Version(clone);
                    return new(ctx.Entity);
                }

                return new(ctx.Entity) { Error = Text.Original("Purge instead") };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
        }

        public virtual Result<T> Purge(DataOperationContext<T> ctx)
        {
            try
            {
                var clone = ctx.Entity.Clone().NullFKObjectsForPersistence();
                using (var db = DbContextFactory.CreateDbContext())
                {
                    db.Set<T>().Where(x => x.Id == clone.Id).ExecuteDelete();
                }
                return new(ctx.Entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
        }

        public virtual T? ReadFromDatabase(IId id)
        {
            using var db = DbContextFactory.CreateDbContext();
            return db.Set<T>().AsNoTracking().Id(id.Id);
        }

        public virtual Result<T> Restore(DataOperationContext<T> ctx)
        {
            try
            {
                var clone = ctx.Entity.Clone();
                if (clone is IDeleted deleted)
                {
                    deleted.IsDeleted = false;
                    using (var db = DbContextFactory.CreateDbContext())
                    {
                        clone = db.Update(clone).Entity;
                        db.SaveChanges();
                    }

                    ctx.Entity.Deleted(clone);
                    ctx.Entity.Version(clone);
                    return new(ctx.Entity);
                }

                return new(ctx.Entity) { Error = Text.Original("Unable to restore") };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { Error = ex.GetMessage() };
            }
        }

        public virtual Result<T> Update(DataOperationContext<T> ctx)
        {
            try
            {
                var clone = ctx.Entity.Clone().NullFKObjectsForPersistence();
                using (var db = DbContextFactory.CreateDbContext())
                {
                    clone = db.Update(clone).Entity;
                    db.SaveChanges();
                }
                ctx.Entity.Version(clone);
                return new(ctx.Entity);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }
        }
    }
}
