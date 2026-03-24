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
        protected readonly IMemoryCache _memoryCache;
        protected readonly IContentCategoryRepository _contentCategoryRepository;
        protected readonly IContentCommentRepository _contentCommentRepository;
        protected readonly IContentRatingRepository _contentRatingRepository;
        protected readonly IContentReactionRepository _contentReactionRepository;
        protected readonly IContentRepository _contentRepository;
        protected readonly IContentTagRepository _contentTagRepository;
        protected readonly IDbContextFactory<OrigamiDbContext> _dbContextFactory;
        protected readonly IValidator<T2> _validator;
        protected readonly Text Text;

        protected HubContentRepository(
            IMemoryCache memoryCache,
            IValidator<T2> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IContentCategoryRepository contentCategoryRepository,
            IContentCommentRepository contentCommentRepository,
            IContentRatingRepository contentRatingRepository,
            IContentReactionRepository contentReactionRepository,
            IContentRepository contentRepository,
            IContentTagRepository contentTagRepository,
            Text text
            )
        {
            _validator = validator;
            _contentCategoryRepository = contentCategoryRepository;
            _contentCommentRepository = contentCommentRepository;
            _contentRatingRepository = contentRatingRepository;
            _contentReactionRepository = contentReactionRepository;
            _contentRepository = contentRepository;
            _contentTagRepository = contentTagRepository;
            _dbContextFactory = dbContextFactory;
            _memoryCache = memoryCache;
            Text = text;
        }

        public virtual string CreatePermission { get; } = string.Empty;
        public virtual string UpdateOtherUsersPermission { get; } = string.Empty;
        public virtual string UpdateOwnPermission { get; } = string.Empty;

        public Task<T2> GetAsync(IId rootId)
        {
            var result = new T2();

            var tasks = new List<Task>
            {
                Task.Run(() => result.Entity = (T1?)_contentRepository.ReadFromCache().Id(rootId.Id)),
                Task.Run(() => result.Categories.AddRange(_contentCategoryRepository.ReadFromCache().Where(x => x.ContentId == rootId.Id))),
                Task.Run(() => result.Comments.AddRange(_contentCommentRepository.ReadFromCache().Where(x => x.ContentId == rootId.Id))),
                Task.Run(() => result.Ratings.AddRange(_contentRatingRepository.ReadFromCache().Where(x => x.ContentId == rootId.Id))),
                Task.Run(() => result.Reactions.AddRange(_contentReactionRepository.ReadFromCache().Where(x => x.ContentId == rootId.Id))),
                Task.Run(() => result.Tags.AddRange(_contentTagRepository.ReadFromCache().Where(x => x.ContentId == rootId.Id))),
            };

            Task.WhenAll(tasks);

            return Task.FromResult(result);
        }

        public Result<T2> Save(T2 root, IId userId)
        {
            if (root.Entity == null) return new(root) { Error = Text.Original("Entity is null"), };

            // hub
            var hub = new Result<T2>(root);

            try
            {
                using var db = _dbContextFactory.CreateDbContext();

                // needs to hit the database for the entity
                var nil = db.Contents.AsNoTracking().Id(root.Entity.Id) == null;
                var permission = nil ? CreatePermission : root.Entity.AuthorId == userId.Id ? UpdateOwnPermission : UpdateOtherUsersPermission;

                // check permissions
                if (UserHasPermission(userId.Id, permission) == false) return new(root) { Info = permission, Error = Text.Original(Text.YouDontHavePermissionForThisFeature), };

                // validate hub
                _validator.ValidateAndThrow(root);

                db.Entry(root.Entity).State = nil ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();

                var m1 = Save(root.Entity, root.Categories);
                var m2 = Save(root.Entity, root.Tags);

                _memoryCache.SaveCache(root.Entity);
                _memoryCache.MergeCache(m1);
                _memoryCache.MergeCache(m2);

                return hub;
            }
            catch (Exception ex)
            {
                return new(root) { Error = ex.GetMessage(), };
            }
        }

        protected virtual bool UserHasPermission(Guid userId, string permission)
        {
            using var db = this._dbContextFactory.CreateDbContext();

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

        private Merge<T> Save<T>(T1 entity, IEnumerable<T> entities) where T : class, IId
        {
            using var db = this._dbContextFactory.CreateDbContext();

            var fresh = from x in db.Set<T>().AsNoTracking() where x.Id == entity.Id select x;
            var merge = fresh.GetMerge(entities);

            merge.Create.Each(x => db.Entry(x).State = EntityState.Added);
            merge.Update.Each(x => db.Entry(x).State = EntityState.Modified);
            merge.Purge.Each(x => db.Entry(x).State = EntityState.Deleted);

            db.SaveChanges();

            return merge;
        }
    }
}
