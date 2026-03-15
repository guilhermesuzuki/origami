using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer0Data<T> : RepositoryBaseLayer<T>,
        ICrud<T>
        where T : class, IId, new()
    {
        protected RepositoryLayer0Data(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
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
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
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

                return new(ctx.Entity) { ErrorMessage = Text.Original("Purge instead") };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }
        }

        public virtual Result<T> Purge(DataOperationContext<T> ctx)
        {
            try
            {
                var clone = ctx.Entity.Clone().NullFKObjectsForPersistence();
                using (var db = DbContextFactory.CreateDbContext())
                {
                    db.Remove(clone);
                    db.SaveChanges();
                }
                return new(ctx.Entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }
        }

        public virtual IQueryable<T> ReadFromDatabase()
        {
            return this.ReadFromDatabase<T>();
        }

        public virtual IQueryable<X> ReadFromDatabase<X>()
            where X : class
        {
            return DbContextFactory.CreateDbContext().Set<X>().AsQueryable();
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

                return new(ctx.Entity) { ErrorMessage = Text.Original("Unable to restore") };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
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
