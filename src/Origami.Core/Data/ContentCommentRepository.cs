using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class ContentCommentRepository :
        RepositoryOuterLayer<OrigamiContentComment>,
        IContentCommentRepository
    {
        protected readonly IValidator<OrigamiContentComment> _validator;
        protected readonly IContentCommentReactionRepository _contentCommentReactionRepository;
        protected readonly IEventRepository _eventRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentCommentRepository(
            IContentCommentReactionRepository contentCommentReactionRepository,
            IEventRepository eventRepository,
            IValidator<OrigamiContentComment> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _contentCommentReactionRepository = contentCommentReactionRepository;
            _eventRepository = eventRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.ModerateComments);
        public override string DeletePermission => nameof(OrigamiRole.ModerateComments);
        public override string PurgePermission => nameof(OrigamiRole.PurgeComments);
        public override string ReadPermission => nameof(OrigamiRole.ViewComments);
        public override string RestorePermission => nameof(OrigamiRole.RestoreComments);
        public override string UpdatePermission => nameof(OrigamiRole.ModerateComments);

        public List<OrigamiContentComment> AllComments(OrigamiContent? entity)
        {
            if (entity != null)
            {
                //comments from a post
                var comments = ReadFromCache().NonDeleted().Where(x => x.ContentId == entity.Id);
                return comments.OrderBy(x => x.IsPinnedBySomeone ? 0 : 1).ThenBy(x => x.DateCreated).ToList();
            }
            return [];
        }

        public List<OrigamiContentComment> CommentsFromProfile(OrigamiSocialProfile entity, bool deleted)
        {
            //comments from a post
            var comments = ReadFromCache().Where(x => x.SocialProfileId == entity.Id);

            //retrieves the deleted ones or not
            if (deleted == false) comments = comments.Where(x => x.IsDeleted == false);

            //returns ordered
            return comments.OrderBy(x => x.PinnedById != null ? 0 : 1).ThenByDescending(x => x.DateCreated).ToList();
        }

        public override Result<OrigamiContentComment> CreateValidation(DataOperationContext<OrigamiContentComment> ctx)
        {
            return new Result<OrigamiContentComment>(ctx.Entity, _validator);
        }

        public long GetComments(OrigamiContent entity)
        {
            var key = entity.KeyForCachingComments();
            return this.MemoryCache.TryGetValue(key, out long x) ? x : 0;
        }

        public Result<OrigamiContentComment> Pin(DataOperationContextFrontEnd<OrigamiContentComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                var profile = db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id)!;
                if (profile.IsModerator)
                {
                    ctx.Entity.PinnedById = ctx.SocialProfile.Id;
                    var hub = base.SmartUpdate(ctx, false);
                    if (hub.Ok == true)
                    {
                        this._eventRepository.SocialProfilePinsComment(ctx.SocialProfile, ctx.Entity);
                    }
                    return hub;
                }
            }
            catch (Exception)
            {
                return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public Result<OrigamiContentComment> Pin(DataOperationContext<OrigamiContentComment> ctx, bool checkPermission)
        {
            try
            {
                if (checkPermission)
                {
                    var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ModerateComments));
                    if (permission.Ok == false) return new(ctx.Entity) { Error = Text.Original(Text.YouDontHavePermissionForThisFeature) };
                }

                ctx.Entity.PinnedByUserId = ctx.User.Id;
                return base.SmartUpdate(ctx, false);
            }
            catch (Exception)
            {
                return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }
        }

        public override void PurgeRelationshipsFromCache(OrigamiContentComment entity)
        {
            base.PurgeRelationshipsFromCache(entity);

            var reactions = from x in _contentCommentReactionRepository.ReadFromCache()
                            where x.CommentId == entity.Id
                            select x;

            reactions.Each(_contentCommentReactionRepository.PurgeCache);
        }

        public override Result<OrigamiContentComment> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiContentComment> ctx)
        {
            var hub = base.PurgeRelationshipsFromDatabase(ctx);
            using var db = DbContextFactory.CreateDbContext();
            hub.RowsAffected += (from x in db.Set<OrigamiContentCommentReaction>().AsNoTracking() where x.CommentId == ctx.Entity.Id select x).ExecuteDelete();
            return hub;
        }

        public override void RefreshCache()
        {
            base.RefreshCache();
            this.RefreshCacheII();
        }

        public Result<OrigamiContentComment> SmartCreate(DataOperationContextFrontEnd<OrigamiContentComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            //figure out how many comments the user has made in 5 minutes
            var commentsMadeByTheUser = ReadFromCache()
                .Where(x => x.SocialProfileId == ctx.SocialProfile.Id)
                .Where(x => x.DateCreated.ToUniversalTime() >= DateTime.UtcNow.AddMinutes(-5))
                .Count();

            //if it's more than 20, timeout
            if (commentsMadeByTheUser >= 20)
            {
                return new(ctx.Entity) { Error = Text.Original(Text.YouMadeTooManyCommentsIn5Minutes) };
            }

            var hub = base.SmartCreate(ctx, false);

            if (hub.Ok == true)
            {
                if (ctx.Entity.ParentId == null)
                {
                    _eventRepository.SocialProfileRepliesToContent(ctx.SocialProfile, ctx.Entity);
                }
                else
                {
                    _eventRepository.SocialProfileRepliesToComment(ctx.SocialProfile, ctx.Entity);
                }
            }

            return hub;
        }

        public Result<OrigamiContentComment> SmartDelete(DataOperationContextFrontEnd<OrigamiContentComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                var profile = db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
                if (profile.IsModerator)
                {
                    var hub = base.SmartDelete(ctx, false);

                    if (hub.Ok == true)
                    {
                        _eventRepository.SocialProfileDeletesComment(ctx.SocialProfile, ctx.Entity);
                    }

                    return hub;
                }
            }
            catch (Exception)
            {
                return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            var comment = ReadFromCache().Id(ctx.Entity.Id);
            if (comment != null)
            {
                if (comment.SocialProfileId == ctx.SocialProfile.Id)
                {
                    var hub = base.SmartDelete(ctx, false);

                    if (hub.Ok == true)
                    {
                        _eventRepository.SocialProfileDeletesComment(ctx.SocialProfile, ctx.Entity);
                    }

                    return hub;
                }
            }

            return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public Result<OrigamiContentComment> SmartUpdate(DataOperationContextFrontEnd<OrigamiContentComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            // user must have created the comment to edit it
            if (ctx.SocialProfile.Id != ctx.Entity.SocialProfileId)
            {
                return new(ctx.Entity) { Error = Text.Original("You cannot edit this comment") };
            }

            var hub = base.SmartUpdate(ctx, false);

            _eventRepository.SocialProfileEditsComment(ctx.SocialProfile, ctx.Entity);

            return hub;
        }

        public Result<OrigamiContentComment> Unpin(DataOperationContextFrontEnd<OrigamiContentComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                var profile = db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id)!;
                if (profile.IsModerator)
                {
                    ctx.Entity.PinnedById = null;
                    var hub = base.SmartUpdate(ctx, false);
                    if (hub.Ok == true)
                    {
                        this._eventRepository.SocialProfileUnpinsComment(ctx.SocialProfile, ctx.Entity);
                    }
                    return hub;
                }
            }
            catch (Exception)
            {
                return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public Result<OrigamiContentComment> Unpin(DataOperationContext<OrigamiContentComment> ctx, bool checkPermission)
        {
            try
            {
                if (checkPermission)
                {
                    var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ModerateComments));
                    if (permission.Ok == false) return new(ctx.Entity) { Error = Text.Original(Text.YouDontHavePermissionForThisFeature) };
                }

                ctx.Entity.PinnedByUserId = null;
                return base.SmartUpdate(ctx, false);
            }
            catch (Exception)
            {
                return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }
        }

        public override Result<OrigamiContentComment> UpdateValidation(DataOperationContext<OrigamiContentComment> ctx)
        {
            return new Result<OrigamiContentComment>(ctx.Entity, _validator);
        }

        protected void RefreshCacheII()
        {
            using var db = DbContextFactory.CreateDbContext();
            var query = from view in db.Set<OrigamiContentComment>().AsNoTracking()
                        where view.IsDeleted == false
                        group view by view.ContentId into g
                        select new { ContentId = g.Key, TotalComments = g.LongCount() };
            var options = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) };

            foreach (var view in query)
            {
                var content = this.MemoryCache.Read<OrigamiContent>().Id(view.ContentId);
                if (content != null)
                {
                    var key = content.KeyForCachingComments();
                    this.MemoryCache.Set(key, view.TotalComments, options);
                }
            }
        }
    }
}
