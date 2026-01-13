using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryBaseLayer<T> :
        ICrud<T>,
        ICache<T>
        where T : class, IId, new()
    {
        protected RepositoryBaseLayer(Text text, IDbContextFactory<OrigamiDbContext> dbContextFactory, IMemoryCache memoryCache, IWebRootPath webRootPath) : base()
        {
            DbContextFactory = dbContextFactory;
            MemoryCache = memoryCache;
            Text = text;
            WebRootPath = webRootPath;
        }

        public IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }
        public string KeyForCaching => typeof(T).KeyForCaching();
        public IMemoryCache MemoryCache { get; }
        public Text Text { get; }
        public IWebRootPath WebRootPath { get; }

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

        public virtual void CreateCache(T entity)
        {
            lock (OrigamiConstants.SyncRoot)
            {
                var value = MemoryCache.GetList<T>(KeyForCaching);
                if (value != null) value.Add(entity); else value = [entity];
                MemoryCache.Set(KeyForCaching, value);
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

        public virtual void PurgeCache(T entity)
        {
            lock (OrigamiConstants.SyncRoot)
            {
                var value = MemoryCache.GetList<T>(KeyForCaching);
                if (value != null)
                {
                    value.RemoveAll(x => x.Id == entity.Id);
                    MemoryCache.Set(KeyForCaching, value);
                }
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

        public virtual void UpdateCache(T entity)
        {
            lock (OrigamiConstants.SyncRoot)
            {
                var value = MemoryCache.GetList<T>(KeyForCaching);
                if (value != null)
                {
                    value.RemoveAll(x => x.Id == entity.Id);
                    value.Add(entity);
                }
                else value = [entity];

                MemoryCache.Set(KeyForCaching, value);
            }
        }

    }
}
