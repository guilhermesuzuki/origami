using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System.Diagnostics;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer3Data<T> :
        RepositoryLayer2Permission<T>,
        IMerge<T>
        where T : class, IId, new()
    {
        protected RepositoryLayer3Data(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public Result Merge(DataOperationContext main, (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) merge)
        {
            var hub = new Result();

            merge.Purge.GetContexts(main).Call(SmartPurge, false).Push(hub);
            merge.Create.GetContexts(main).Call(SmartCreate, false).Push(hub);
            merge.Update.GetContexts(main).Call(SmartUpdate, false).Push(hub);

            return hub;
        }

        public Result MergeCache((IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) merge)
        {
            merge.Purge.Each(this.PurgeCache);
            merge.Update.Each(this.UpdateCache);
            merge.Create.Each(this.CreateCache);

            return new();
        }

        public void PurgeChildrenFromCache(T entity)
        {
            this.ReadFromCache().GetAllChildren(entity).Each(this.PurgeCache);
        }

        public Result<T> PurgeChildrenFromDatabase(DataOperationContext<T> main)
        {
            var mainHub = new Result<T>(main.Entity);
            var children = this.ReadFromDatabase().GetAllChildren(main.Entity);
            foreach (var child in children)
            {
                var ctx = new DataOperationContext<T>(main.User, child);
                var hub = this.SmartPurge(ctx, false);
                if (hub.Ok == false) return hub;
                mainHub.Pull(hub);
            }
            return mainHub;
        }

        public virtual void PurgeRelationshipsFromCache(T entity)
        {
            return;
        }

        public virtual Result<T> PurgeRelationshipsFromDatabase(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual List<T> ReadFromCache()
        {
            return this.ReadFromCache<T>();
        }

        public virtual List<X> ReadFromCache<X>() where X : class, new()
        {
            var key = typeof(X).KeyForCaching();
            var timestamp = Stopwatch.GetTimestamp();

            try
            {
                //race condition
                if (MemoryCache.Get(key) == null)
                {
                    lock (OrigamiConstants.SyncRoot)
                    {
                        if (MemoryCache.Get(key) == null)
                        {
                            var list = this.ReadFromDatabase<X>().ToList();
                            MemoryCache.Set(key, list);
                        }
                    }
                }
                return MemoryCache.GetList<X>(key) ?? [];
            }
            finally
            {
                var elapsedTime = Stopwatch.GetElapsedTime(timestamp);
                Console.ForegroundColor = elapsedTime.Milliseconds >= 200 ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine($"{key} obtained in {elapsedTime}");
            }
        }

        public Result<T> SmartCreate(DataOperationContext<T> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanCreate(ctx);
                if (permission is { Ok: false }) return permission;
            }

            var validation = this.CreateValidation(ctx);
            if (validation.Ok == false) return validation;

            ctx.Entity.SetDateCreated(DateTime.UtcNow);

            return Create(ctx).OnSuccess(() => CreateCache(ctx.Entity));
        }

        public Result<T> SmartDelete(DataOperationContext<T> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanDelete(ctx);
                if (permission is { Ok: false }) return permission;
            }

            var validation = this.DeleteValidation(ctx);
            if (validation.Ok == false) return validation;

            if (ctx.Entity is IDeleted deleted)
            {
                deleted.IsDeleted = true;
                return Update(ctx).OnSuccess(() => UpdateCache(ctx.Entity));
            }

            return new(ctx.Entity) { ErrorMessage = Text.Original("Purge instead") };
        }

        public Result<T> SmartPublish(DataOperationContext<T> ctx, bool checkPermission)
        {
            try
            {
                if (checkPermission)
                {
                    var permission = CanPublish(ctx);
                    if (permission.Ok == false) return permission;
                }
                if (ctx.Entity is IPublished published)
                {
                    var db = ReadFromDatabase().Id(ctx.Entity.Id) as IPublished;

                    if (db == null)
                    {
                        return new(ctx.Entity) { ErrorMessage = Text.Original("{0} does NOT exist", published.GetType().Name) };
                    }
                    if (db.IsPublished)
                    {
                        return new(ctx.Entity) { ErrorMessage = Text.Original("{0} is already published", published.GetType().Name) };
                    }

                    published.IsPublished = true;
                    published.DatePublished = DateTime.UtcNow;
                    SmartUpdate(ctx, false);

                    return new(ctx.Entity);
                }
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }

            throw new NotImplementedException();
        }

        public virtual Result<T> SmartPurge(DataOperationContext<T> ctx, bool checkPermission)
        {
            var hub = new Result<T>(ctx.Entity);
            try
            {
                if (checkPermission)
                {
                    var permission = this.CanPurge(ctx);
                    if (permission.Ok == false) return permission;
                }

                var validation = this.PurgeValidation(ctx);
                if (validation.Ok == false) return validation;

                using var db = this.DbContextFactory.CreateDbContext();
                hub.Pull(this.PurgeChildrenFromDatabase(ctx));
                hub.Pull(this.PurgeRelationshipsFromDatabase(ctx));
                this.Purge(ctx).Push(hub);

                if (hub.Ok)
                {
                    this.PurgeCache(ctx.Entity);
                    this.PurgeChildrenFromCache(ctx.Entity);
                    this.PurgeRelationshipsFromCache(ctx.Entity);
                }
            }
            catch (Exception ex)
            {
                hub.ErrorMessage = ex.GetMessage();
            }

            return hub;
        }

        public virtual Result<T> SmartRestore(DataOperationContext<T> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanRestore(ctx);
                if (permission is { Ok: false }) return permission;
            }

            if (ctx.Entity is IDeleted deleted)
            {
                deleted.IsDeleted = false;
                return Update(ctx).OnSuccess(() => UpdateCache(ctx.Entity));
            }

            return new(ctx.Entity) { ErrorMessage = Text.Original("Unable to restore") };
        }

        public Result<T> SmartSave(DataOperationContext<T> ctx, bool checkPermission)
        {
            if (ctx.Entity is INew { New: true })
            {
                return this.SmartCreate(ctx, checkPermission);
            }

            if (ctx.Entity is INew { New: false })
            {
                return this.SmartUpdate(ctx, checkPermission);
            }

            var fresh = ReadFromDatabase().FirstOrDefault(x => x.Id == ctx.Entity.Id);
            return fresh == null ? this.SmartCreate(ctx, checkPermission) : this.SmartUpdate(ctx, checkPermission);
        }

        public Result<T> SmartUnpublish(DataOperationContext<T> ctx, bool checkPermission)
        {
            try
            {
                if (checkPermission)
                {
                    var permission = CanUnpublish(ctx);
                    if (permission.Ok == false) return permission;
                }
                if (ctx.Entity is IPublished published)
                {
                    var db = ReadFromDatabase().Id(ctx.Entity.Id) as IPublished;
                    if (db == null)
                    {
                        return new(ctx.Entity) { ErrorMessage = Text.Original("{0} does NOT exist", published.GetType().Name) };
                    }
                    if (db.IsPublished == false)
                    {
                        return new(ctx.Entity) { ErrorMessage = Text.Original("{0} is already unpublished", published.GetType().Name) };
                    }
                    published.IsPublished = false;
                    SmartUpdate(ctx, false);
                    return new(ctx.Entity);
                }
            }
            catch (Exception ex)
            {
                return new(ctx.Entity) { ErrorMessage = ex.GetMessage() };
            }
            throw new NotImplementedException();
        }

        public Result<T> SmartUpdate(DataOperationContext<T> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanUpdate(ctx);
                if (permission is { Ok: false }) return permission;
            }

            var validation = this.UpdateValidation(ctx);
            if (validation.Ok == false) return validation;

            ctx.Entity.SetDateModified(DateTime.UtcNow);

            return Update(ctx).OnSuccess(() => UpdateCache(ctx.Entity));
        }
    }
}
