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

        public virtual string ReadPermission { get; } = string.Empty;

        public virtual string CreatePermission { get; } = string.Empty;
        public virtual string DeleteOtherUsersPermission { get; } = string.Empty;
        public virtual string DeleteOwnPermission { get; } = string.Empty;
        public virtual string UpdateOtherUsersPermission { get; } = string.Empty;
        public virtual string UpdateOwnPermission { get; } = string.Empty;
        public virtual string RestorePermission { get; } = string.Empty;
        public virtual string PurgePermission { get; } = string.Empty;
        public virtual string PublishOtherUsersPermission { get; } = string.Empty;
        public virtual string PublishOwnPermission { get; } = string.Empty;
        public virtual string UnpublishOtherUsersPermission { get; } = string.Empty;
        public virtual string UnpublishOwnPermission { get; } = string.Empty;
        public virtual string PromoteToFrontPagePermission { get; } = string.Empty;
        public virtual string DemoteFromFrontPagePermission { get; } = string.Empty;

        public Result CanRead(IId userId)
        {
            using var db = _dbContextFactory.CreateDbContext();

            // check permissions
            if (UserHasPermission(db, userId.Id, ReadPermission) == false) return new() { Info = ReadPermission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

            // success
            return new() { Info = ReadPermission, Success = Text.Original(Text.OperationCompletedSuccessfully), };
        }

        public Result<T2> Delete(T2 root, IId userId)
        {
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
                _memoryCache.SaveCache(entity);

                // returns success
                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
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
                Task.Run(() => {
                    result.Entity = this.GetEntity(rootId) ?? Activator.CreateInstance<T1>();
                    result.Parent = this.GetParent(result.Entity);
                }),
                Task.Run(() => result.Children.AddRange(this.GetChildren(result.Entity))),
                Task.Run(() => result.Categories.AddRange(this.GetEntities<OrigamiContentCategory>(rootId))),
                Task.Run(() => result.Comments.AddRange(this.GetEntities<OrigamiContentComment>(rootId))),
                Task.Run(() => result.Ratings.AddRange(this.GetEntities<OrigamiContentRating>(rootId))),
                Task.Run(() => result.Reactions.AddRange(this.GetEntities<OrigamiContentReaction>(rootId))),
                Task.Run(() => result.Tags.AddRange(this.GetEntities<OrigamiContentTag>(rootId))),
            };

            Task.WhenAll(tasks);

            return result;
        }

        public Result<T2> Publish(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var permission = root.Entity.AuthorId == userId.Id ? PublishOwnPermission : PublishOtherUsersPermission;

                // check permissions
                if (UserHasPermission(db, userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // marks as published
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(
                    s =>
                    {
                        s.SetProperty(x => x.IsPublished, true);
                        s.SetProperty(x => x.DatePublished, DateTime.UtcNow);
                    });

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id);

                // needs to update cache
                _memoryCache.SaveCache(entity);

                // returns success
                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
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
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id);

                // needs to update cache
                _memoryCache.SaveCache(entity);

                // returns success
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

        public Result<T2> Unpublish(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var permission = root.Entity.AuthorId == userId.Id ? PublishOwnPermission : PublishOtherUsersPermission;

                // check permissions
                if (UserHasPermission(db, userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // marks as unpublished
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(
                    s =>
                    {
                        s.SetProperty(x => x.IsPublished, false);
                        s.SetProperty(x => x.DatePublished, (DateTime?)null);
                    });

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id);

                // needs to update cache
                _memoryCache.SaveCache(entity);

                // returns success
                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }

        public Result<T2> PromoteToFrontPage(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // check permissions
                if (UserHasPermission(db, userId.Id, PromoteToFrontPagePermission) == false) return new(root) { Info = PromoteToFrontPagePermission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                var entity = db.Set<OrigamiPage>().AsNoTracking().Id(root.Entity.Id);
                if (entity != null)
                {
                    // validate whether the entity is top-level page (use fluent validator for this?)
                    _validator.ValidateAndThrow(root);

                    // previous front-page, if exists, should be unmarked
                    var frontpage = (from page in db.Set<OrigamiPage>().AsNoTracking() where page.IsFrontPage select page).FirstOrDefault();
                    if (frontpage != null && frontpage.Id != root.Entity.Id)
                    {
                        this.DemoteFromFrontPage(this.Get(frontpage), userId);
                    }

                    // promotes to front-page
                    db.Set<OrigamiPage>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsFrontPage, true));

                    // needs to hit the database again for the entity that was updated
                    entity = db.Set<OrigamiPage>().AsNoTracking().Id(root.Entity.Id);

                    // needs to update cache
                    _memoryCache.SaveCache(entity);

                    // returns success
                    return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
                }

                return new(root) { Error = Text.Original("Page not found"), };
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }

        public Result<T2> DemoteFromFrontPage(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // check permissions
                if (UserHasPermission(db, userId.Id, DemoteFromFrontPagePermission) == false) return new(root) { Info = DemoteFromFrontPagePermission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // marks as published
                db.Set<OrigamiPage>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsFrontPage, false));

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id);

                // needs to update cache
                _memoryCache.SaveCache(entity);

                // returns success
                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
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
            return _dbContextFactory.ReadFromCache<X>(this._memoryCache).Where(x => x.ContentId == id.Id).ToList();
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
            return _dbContextFactory.ReadFromCache<T1>(this._memoryCache).Id(id.Id);
        }

        /// <summary>
        /// Retrieves the parent entity of type T1 corresponding to the specified parent identifier.
        /// </summary>
        /// <param name="parentId">An object that provides the parent identifier used to locate the parent entity. Cannot be null.</param>
        /// <returns>The parent entity of type T1 if found; otherwise, null.</returns>
        protected T1? GetParent(IParentIdNull parentId)
        {
            return _dbContextFactory.ReadFromCache<T1>(this._memoryCache).Id(parentId.ParentId);
        }

        /// <summary>
        /// Retrieves a list of child entities associated with the specified parent identifier.
        /// </summary>
        /// <param name="id">The identifier of the parent entity whose children are to be retrieved. Cannot be null.</param>
        /// <returns>A list of child entities of type T1 that have the specified parent identifier. Returns an empty list if no
        /// children are found.</returns>
        protected List<T1> GetChildren(IId id)
        {
            return _dbContextFactory.ReadFromCache<T1>(this._memoryCache).Where(x => x.ParentId == id.Id).ToList();
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
