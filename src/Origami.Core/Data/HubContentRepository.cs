using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class HubContentRepository<T1, T2> : IHubContentRepository<T2>
        where T1 : OrigamiContent
        where T2 : class, IHubContent<T1>, new()
    {
        protected readonly IDbContextFactory<OrigamiDbContext> _dbContextFactory;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IValidator<T2> _validator;
        protected readonly Text Text;

        protected HubContentRepository(
            IMemoryCache memoryCache,
            IValidator<T2> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            )
        {
            _validator = validator;
            _dbContextFactory = dbContextFactory;
            _memoryCache = memoryCache;
            Text = text;
        }

        public virtual string CreatePermission { get; } = string.Empty;
        public virtual string UpdateOtherUsersPermission { get; } = string.Empty;
        public virtual string UpdateOwnPermission { get; } = string.Empty;

        public virtual string DeleteOtherUsersPermission { get; } = string.Empty;
        public virtual string DeleteOwnPermission { get; } = string.Empty;

        public virtual string RestorePermission { get; } = string.Empty;
        public virtual string PurgePermission { get; } = string.Empty;

        public Result<T2> Delete(T2 root, IId userId)
        {
            // hub
            var hub = new Result<T2>(root);

            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var permission = root.Entity.AuthorId == userId.Id ? DeleteOwnPermission : DeleteOtherUsersPermission;

                // check permissions
                if (UserHasPermission(db, userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // marks as deleted
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsDeleted, true));

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id);

                // needs to update cache
                _memoryCache.SaveCache(entity as OrigamiContent);

                hub.Success = Text.Original(Text.OperationCompletedSuccessfully);

                return hub;
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }

        public T2 Get(IId rootId)
        {
            var result = new T2();

            var tasks = new List<Task>
            {
                Task.Run(() => result.Entity = this.GetEntity(rootId) ?? Activator.CreateInstance<T1>()),
                Task.Run(() => result.Categories.AddRange(this.GetEntities<OrigamiContentCategory>(rootId))),
                Task.Run(() => result.Comments.AddRange(this.GetEntities<OrigamiContentComment>(rootId))),
                Task.Run(() => result.Ratings.AddRange(this.GetEntities<OrigamiContentRating>(rootId))),
                Task.Run(() => result.Reactions.AddRange(this.GetEntities<OrigamiContentReaction>(rootId))),
                Task.Run(() => result.Tags.AddRange(this.GetEntities<OrigamiContentTag>(rootId))),
            };

            Task.WhenAll(tasks);

            return result;
        }

        public Result<T2> Purge(T2 root, IId userId)
        {
            throw new NotImplementedException();
        }

        public Result<T2> Restore(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var permission = root.Entity.AuthorId == userId.Id ? DeleteOwnPermission : DeleteOtherUsersPermission;

                // check permissions
                if (UserHasPermission(db, userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // marks as deleted
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsDeleted, false));

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id) as OrigamiContent;

                // needs to update cache
                _memoryCache.SaveCache(entity);

                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }

        public Result<T2> Save(T2 root, IId userId)
        {
            // hub
            var hub = new Result<T2>(root);

            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var nil = db.Contents.AsNoTracking().Id(root.Entity.Id) == null;
                var permission = nil ? CreatePermission : root.Entity.AuthorId == userId.Id ? UpdateOwnPermission : UpdateOtherUsersPermission;

                // check permissions
                if (UserHasPermission(db, userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // validate hub
                _validator.ValidateAndThrow(root);

                db.Entry(root.Entity).State = nil ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();

                var m1 = Save(db, root.Entity, root.Categories);
                var m2 = Save(db, root.Entity, root.Tags);

                _memoryCache.SaveCache((OrigamiContent)root.Entity);
                _memoryCache.SaveCache(m1);
                _memoryCache.SaveCache(m2);

                hub.Success = Text.Original(Text.OperationCompletedSuccessfully);

                return hub;
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }

        /// <summary>
        /// Retrieves a list of entities of the specified type that match the given content identifier.
        /// </summary>
        /// <remarks>The returned entities are not tracked by the context. This method is typically used
        /// for read-only operations.</remarks>
        /// <typeparam name="X">The type of entity to retrieve. Must implement both IId and IContentId.</typeparam>
        /// <param name="id">The content identifier used to filter the entities. Only entities with a matching ContentId are returned.</param>
        /// <returns>A list of entities of type X whose ContentId matches the specified identifier. The list is empty if no
        /// matching entities are found.</returns>
        protected List<X> GetEntities<X>(IId id) where X : class, IId, IContentId
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.ReadFromCache<X>(this._memoryCache).Where(x => x.ContentId == id.Id).ToList();
        }

        /// <summary>
        /// Retrieves an entity of type T1 with the specified identifier from the database without tracking changes.
        /// </summary>
        /// <remarks>The returned entity is not tracked by the context, so changes to it will not be
        /// persisted unless it is attached to the context and explicitly updated.</remarks>
        /// <param name="id">An object that provides the unique identifier of the entity to retrieve. Cannot be null.</param>
        /// <returns>The entity of type T1 that matches the specified identifier, or null if no such entity exists.</returns>
        protected T1? GetEntity(IId id)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return db.ReadFromCache<T1>(this._memoryCache).Id(id.Id);
        }

        protected virtual bool UserHasPermission(OrigamiDbContext db, Guid userId, string permission)
        {
            var user = db.Users.AsNoTracking().Id(userId);
            if (user == null) return false;
            if (user.IsDeleted) return false;
            if (user.IsBlocked) return false;

            var query = from us in db.Users
                        join ur in db.UserRoles on us.Id equals ur.UserId
                        join ro in db.Roles on ur.RoleId equals ro.Id
                        join rr in db.RightRoles on ro.Id equals rr.RoleId
                        join ri in db.Rights on rr.RightId equals ri.Id
                        where us.IsDeleted == false
                        where ro.IsDeleted == false
                        where us.Id == userId && ri.Name == permission
                        select 1;

            return query.Any();
        }

        private Merge<T> Save<T>(OrigamiDbContext db, T1 entity, IEnumerable<T> entities) where T : class, IId, IContentId
        {
            var fresh = from x in db.Set<T>().AsNoTracking() where x.ContentId == entity.Id select x;
            var merge = fresh.GetMerge(entities.ToList());

            merge.Create.Each(x => db.Entry(x).State = EntityState.Added);
            merge.Update.Each(x => db.Entry(x).State = EntityState.Modified);
            merge.Purge.Each(x => db.Entry(x).State = EntityState.Deleted);

            db.SaveChanges();

            return merge;
        }
    }
}
