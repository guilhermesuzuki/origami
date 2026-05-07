using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class HubContentRepository<T1, T2> : IHubContentRepository<T2>
        where T1 : OrigamiContent
        where T2 : class, IHubContent<T1>, new()
    {
        protected readonly IDbContextFactory<OrigamiDbContext> _dbContextFactory;
        protected readonly IMyMemoryCache _memoryCache;
        protected readonly IValidator<T2> _validator;
        protected readonly Text Text;

        protected HubContentRepository(
            IMyMemoryCache memoryCache,
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
        public virtual string DeleteOtherUsersPermission { get; } = string.Empty;
        public virtual string DeleteOwnPermission { get; } = string.Empty;
        public virtual string DemoteFromFrontPagePermission { get; } = string.Empty;
        public virtual string PromoteToFrontPagePermission { get; } = string.Empty;
        public virtual string PublishOtherUsersPermission { get; } = string.Empty;
        public virtual string PublishOwnPermission { get; } = string.Empty;
        public virtual string PurgePermission { get; } = string.Empty;
        public virtual string ReadPermission { get; } = string.Empty;
        public virtual string RestorePermission { get; } = string.Empty;
        public virtual string UnpublishOtherUsersPermission { get; } = string.Empty;
        public virtual string UnpublishOwnPermission { get; } = string.Empty;
        public virtual string UpdateOtherUsersPermission { get; } = string.Empty;
        public virtual string UpdateOwnPermission { get; } = string.Empty;

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

                //private scope
                {
                    var fresh = (from a in db.Set<T1>().AsNoTracking() where a.Id == root.Entity.Id select a.IsDeleted).ToList();
                    if (fresh.Any() == false)
                    {
                        return new(root) { Error = Text.Original("Content must exist"), };
                    }
                    if (fresh.Any(f => f == true))
                    {
                        // TODO: add this to resx files
                        return new(root) { Error = Text.Original("Content is already deleted"), };
                    }
                }

                // marks as deleted
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsDeleted, true));

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id)!;

                // needs to update cache
                _memoryCache.Save(entity as OrigamiContent);

                this.History(db, root.Entity, DateTime.UtcNow, "Content deleted", userId);

                root.Entity.IsDeleted = entity.IsDeleted;
                root.Entity.Version(entity);

                // returns success
                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
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
                _memoryCache.Save(entity as OrigamiContent);

                this.History(db, root.Entity, DateTime.UtcNow, "Content demoted from front page", userId);

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

            var entity = this.GetEntity(rootId);
            if (entity != null)
            {
                result.Entity = entity;
            }
            else
            {
                result.Entity.Id = Guid.Empty;
            }

            result.Parent = this.GetParent(result.Entity);
            result.Children.AddRange(this.GetChildren(result.Entity));
            result.Categories.AddRange(this.GetEntities<OrigamiContentCategory>(rootId));
            result.Comments.AddRange(this.GetEntities<OrigamiContentComment>(rootId));
            result.Ratings.AddRange(this.GetEntities<OrigamiContentRating>(rootId));
            result.Reactions.AddRange(this.GetEntities<OrigamiContentReaction>(rootId));
            result.Tags.AddRange(this.GetEntities<OrigamiContentTag>(rootId));
            result.Histories.AddRange(this.GetEntities<OrigamiContentHistory>(rootId));

            return result;
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
                    // only top-level pages can be promoted to front-page
                    if (entity.ParentId != null) return new(root) { Error = Text.Original("Only top-level pages can be promoted to front page"), };

                    // previous front-page, if exists, should be unmarked
                    var frontpage = (from page in db.Set<OrigamiPage>().AsNoTracking() where page.IsFrontPage select page).FirstOrDefault();
                    if (frontpage != null && frontpage.Id == root.Entity.Id)
                    {
                        return new(root) { Error = Text.Original("Page is already front-page"), };
                    }
                    if (frontpage != null)
                    {
                        var demote = this.DemoteFromFrontPage(this.Get(frontpage), userId);
                        if (demote.Ok == false) return demote;
                    }

                    // promotes to front-page
                    db.Set<OrigamiPage>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsFrontPage, true));

                    // needs to hit the database again for the entity that was updated
                    entity = db.Set<OrigamiPage>().AsNoTracking().Id(root.Entity.Id);

                    // needs to update cache
                    _memoryCache.Save(entity as OrigamiContent);

                    this.History(db, root.Entity, DateTime.UtcNow, "Content promoted to front page", userId);

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

        public Result<T2> Publish(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var permission = root.Entity.AuthorId == userId.Id ? PublishOwnPermission : PublishOtherUsersPermission;

                // check permissions
                if (UserHasPermission(db, userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                //private scope
                {
                    var fresh = (from a in db.Set<T1>().AsNoTracking() where a.Id == root.Entity.Id select a.IsPublished).ToList();
                    if (fresh.Any() == false)
                    {
                        return new(root) { Error = Text.Original("Content must exist"), };
                    }
                    if (fresh.Any(f => f == true))
                    {
                        // TODO: add this to resx files
                        return new(root) { Error = Text.Original("Content is already published"), };
                    }
                }

                // marks as published
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(
                    s =>
                    {
                        s.SetProperty(x => x.IsPublished, true);
                        s.SetProperty(x => x.DatePublished, DateTime.UtcNow);
                    });

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id)!;

                // needs to update cache
                _memoryCache.Save(entity as OrigamiContent);

                this.History(db, root.Entity, DateTime.UtcNow, "Content published", userId);

                root.Entity.IsPublished = entity.IsPublished;
                root.Entity.DatePublished = entity.DatePublished;
                root.Entity.Version(entity);

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
            using var db = _dbContextFactory.CreateDbContext();

            // check permissions
            if (UserHasPermission(db, userId.Id, PurgePermission) == false) return new(root) { Info = PurgePermission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

            //first, it needs to purge the relationships
            this.PurgeKids(root, userId);

            var commentReactions = from a in db.Set<OrigamiContentCommentReaction>().AsNoTracking()
                                   join b in db.Set<OrigamiContentComment>().AsNoTracking() on a.CommentId equals b.Id
                                   where b.ContentId == root.Entity.Id
                                   select a;

            var comments = from a in db.Set<OrigamiContentComment>().AsNoTracking() where a.ContentId == root.Entity.Id select a;
            var categories = from a in db.Set<OrigamiContentCategory>().AsNoTracking() where a.ContentId == root.Entity.Id select a;
            var histories = from a in db.Set<OrigamiContentHistory>().AsNoTracking() where a.ContentId == root.Entity.Id select a;
            var ratings = from a in db.Set<OrigamiContentRating>().AsNoTracking() where a.ContentId == root.Entity.Id select a;
            var reactions = from a in db.Set<OrigamiContentReaction>().AsNoTracking() where a.ContentId == root.Entity.Id select a;
            var tags = from a in db.Set<OrigamiContentTag>().AsNoTracking() where a.ContentId == root.Entity.Id select a;
            var entity = from a in db.Set<T1>().AsNoTracking() where a.Id == root.Entity.Id select a;

            _memoryCache.Purge(commentReactions);
            _memoryCache.Purge(comments);
            _memoryCache.Purge(categories);
            _memoryCache.Purge(histories);
            _memoryCache.Purge(ratings);
            _memoryCache.Purge(reactions);
            _memoryCache.Purge(tags);
            _memoryCache.Purge(entity.FirstOrDefault());

            commentReactions.ExecuteDelete();
            comments.ExecuteDelete();
            categories.ExecuteDelete();
            histories.ExecuteDelete();
            ratings.ExecuteDelete();
            reactions.ExecuteDelete();
            tags.ExecuteDelete();
            entity.ExecuteDelete();

            return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
        }

        public Result<T2> PurgeKids(T2 root, IId userId)
        {
            var hub = new Result<T2>(root);
            var db = this._dbContextFactory.CreateDbContext();
            var kids = db.Set<T1>().AsNoTracking().Where(x => x.ParentId == root.Entity.Id).ToList();

            foreach (var child in kids)
            {
                var anotherRoot = new T2() { Entity = child };
                this.PurgeKids(anotherRoot, userId).Push(hub);
                this.Purge(anotherRoot, userId).Push(hub);
            }

            return hub;
        }

        public Result<T2> Restore(T2 root, IId userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // check permissions
                if (UserHasPermission(db, userId.Id, RestorePermission) == false) return new(root) { Info = RestorePermission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                //private scope
                {
                    var fresh = (from a in db.Set<T1>().AsNoTracking() where a.Id == root.Entity.Id select a.IsDeleted).ToList();
                    if (fresh.Any() == false)
                    {
                        return new(root) { Error = Text.Original("Content must exist"), };
                    }
                    if (fresh.Any(f => f == false))
                    {
                        // TODO: add this to resx files
                        return new(root) { Error = Text.Original("Content is already restored"), };
                    }
                }

                // marks as undeleted
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(s => s.SetProperty(x => x.IsDeleted, false));

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id)!;

                // needs to update cache
                _memoryCache.Save(entity as OrigamiContent);

                this.History(db, root.Entity, DateTime.UtcNow, "Content restored", userId);

                root.Entity.IsDeleted = entity.IsDeleted;
                root.Entity.Version(entity);

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

                if (nil)
                {
                    root.Entity.DateCreated = DateTime.UtcNow;
                }
                else
                {
                    root.Entity.DateModified = DateTime.UtcNow;
                }

                root.SetSlug<T1, T2>();

                // validate hub
                _validator.ValidateAndThrow(root);

                db.Entry(root.Entity).State = nil ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();

                var m1 = Save(db, root.Entity, root.Categories);
                var m2 = Save(db, root.Entity, root.Tags);

                _memoryCache.Save(root.Entity as OrigamiContent);
                _memoryCache.SaveCache(m1);
                _memoryCache.SaveCache(m2);

                this.History(db,
                    root.Entity,
                    nil ? root.Entity.DateCreated : root.Entity.DateModified.GetValueOrDefault(),
                    nil ? "Content created" : "Content saved",
                    userId);

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

                //private scope
                {
                    var fresh = (from a in db.Set<T1>().AsNoTracking() where a.Id == root.Entity.Id select a.IsPublished).ToList();
                    if (fresh.Any() == false)
                    {
                        return new(root) { Error = Text.Original("Content must exist"), };
                    }
                    if (fresh.Any(f => f == false))
                    {
                        // TODO: add this to resx files
                        return new(root) { Error = Text.Original("Content is already unpublished"), };
                    }
                }

                // marks as unpublished
                db.Set<T1>().AsNoTracking().Where(x => x.Id == root.Entity.Id).ExecuteUpdate(
                    s =>
                    {
                        s.SetProperty(x => x.IsPublished, false);
                        s.SetProperty(x => x.DatePublished, (DateTime?)null);
                    });

                // needs to hit the database for the entity
                var entity = db.Set<T1>().AsNoTracking().Id(root.Entity.Id)!;

                // needs to update cache
                _memoryCache.Save(entity as OrigamiContent);

                this.History(db, root.Entity, DateTime.UtcNow, "Content unpublished", userId);

                root.Entity.IsPublished = entity.IsPublished;
                root.Entity.DatePublished = entity.DatePublished;
                root.Entity.Version(entity);

                // returns success
                return new(root) { Success = Text.Original(Text.OperationCompletedSuccessfully), };
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }
        /// <summary>
        /// Retrieves a list of child entities associated with the specified parent identifier.
        /// </summary>
        /// <param name="id">The identifier of the parent entity whose children are to be retrieved. Cannot be null.</param>
        /// <returns>A list of child entities of type T1 that have the specified parent identifier. Returns an empty list if no
        /// children are found.</returns>
        protected List<T1> GetChildren(IId id)
        {
            return _memoryCache.Read<T1>().Where(x => x.ParentId == id.Id).ToList();
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
            return _memoryCache.Read<X>().Where(x => x.ContentId == id.Id).ToList();
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
            return _memoryCache.Read<T1>().Id(id.Id);
        }

        /// <summary>
        /// Retrieves the parent entity of type T1 corresponding to the specified parent identifier.
        /// </summary>
        /// <param name="parentId">An object that provides the parent identifier used to locate the parent entity. Cannot be null.</param>
        /// <returns>The parent entity of type T1 if found; otherwise, null.</returns>
        protected T1? GetParent(IParentIdNull parentId)
        {
            return _memoryCache.Read<T1>().Id(parentId.ParentId);
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

        private Result History(OrigamiDbContext db, IId entity, DateTime timestamp, string description, IId author)
        {
            var history = new OrigamiContentHistory()
            {
                ContentId = entity.Id,
                DateCreated = timestamp,
                Description = description,
                AuthorId = author.Id,
            };

            db.Entry(history).State = EntityState.Added;
            db.SaveChanges();

            this._memoryCache.CreateCache(history);

            return new();
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
