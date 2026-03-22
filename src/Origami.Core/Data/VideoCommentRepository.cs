using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoCommentRepository :
        RepositoryOuterLayer<OrigamiVideoComment>,
        IVideoCommentRepository
    {
        protected readonly IValidator<OrigamiVideoComment> _validator;
        protected readonly ISocialProfileRepository _socialProfileRepository;
        protected readonly IVideoCommentReactionRepository _videoCommentReactionRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoCommentRepository(
            IValidator<OrigamiVideoComment> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            ISocialProfileRepository socialProfileRepository,
            IVideoCommentReactionRepository videoCommentReactionRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _socialProfileRepository = socialProfileRepository;
            _videoCommentReactionRepository = videoCommentReactionRepository;
        }

        public override string DeletePermission => nameof(OrigamiRole.ModerateComments);
        public override string PurgePermission => nameof(OrigamiRole.PurgeComments);
        public override string ReadPermission => nameof(OrigamiRole.ViewComments);
        public override string RestorePermission => nameof(OrigamiRole.RestoreComments);

        public List<OrigamiVideoComment> AllComments(OrigamiVideo? entity)
        {
            if (entity != null)
            {
                //comments from a video
                var comments = ReadFromCache().NonDeleted().Where(x => x.VideoId == entity.Id);
                return comments.OrderBy(x => x.PinnedById != null ? 0 : 1).ThenBy(x => x.DateCreated).ToList();
            }
            return [];
        }

        public List<OrigamiVideoComment> CommentsFromProfile(OrigamiSocialProfile entity, bool deleted)
        {
            //comments from a post
            var comments = ReadFromCache().Where(x => x.SocialProfileId == entity.Id);
            //retrieves the deleted ones or not
            if (deleted == false) comments = comments.Where(x => x.IsDeleted == false);
            //returns ordered
            return comments.OrderBy(x => x.PinnedById != null ? 0 : 1).ThenByDescending(x => x.DateCreated).ToList();
        }

        public Result<OrigamiVideoComment> SmartCreate(DataOperationContextFrontEnd<OrigamiVideoComment> ctx)
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

            return base.SmartCreate(ctx, false);
        }

        public override Result<OrigamiVideoComment> CreateValidation(DataOperationContext<OrigamiVideoComment> ctx)
        {
            return new Result<OrigamiVideoComment>(ctx.Entity, _validator);
        }

        public Result<OrigamiVideoComment> SmartDelete(DataOperationContextFrontEnd<OrigamiVideoComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                var profile = db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
                if (profile.IsModerator)
                {
                    return base.SmartDelete(ctx, false);
                }
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            var comment = ReadFromCache().Id(ctx.Entity.Id);
            if (comment != null)
            {
                if (comment.SocialProfileId == ctx.SocialProfile.Id)
                {
                    return base.SmartDelete(ctx, false);
                }
            }

            return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public async Task<List<VideoCommentTotal>> FastRead()
        {
            using var db = DbContextFactory.CreateDbContext();
            var sql = @"SELECT v.Id as VideoId, (SELECT COUNT_BIG(1) FROM dbo.oi_VideoComments vc WHERE vc.VideoId = v.Id AND vc.IsDeleted = 0) as TotalComments FROM dbo.oi_Videos v";
            return await db.Database.SqlQueryRaw<VideoCommentTotal>(sql).ToListAsync();
        }

        public long GetComments(OrigamiVideo entity)
        {
            var key = entity.KeyForCachingComments();
            if (key.Has() == true)
            {
                var value = MemoryCache.Get(key);
                if (value is long l) return l;
            }
            return 0;
        }

        public Result<OrigamiVideoComment> Pin(DataOperationContextFrontEnd<OrigamiVideoComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                var profile = db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
                if (profile.IsModerator)
                {
                    ctx.Entity.PinnedById = ctx.SocialProfile.Id;
                    return base.SmartUpdate(ctx, false);
                }
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public override void PurgeRelationshipsFromCache(OrigamiVideoComment entity)
        {
            base.PurgeRelationshipsFromCache(entity);

            var reactions = from x in _videoCommentReactionRepository.ReadFromCache()
                            where x.CommentId == entity.Id
                            select x;

            reactions.Each(this._videoCommentReactionRepository.PurgeCache);
        }

        public override Result<OrigamiVideoComment> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiVideoComment> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();
            var hub = base.PurgeRelationshipsFromDatabase(ctx);
            var reactions = (from x in db.Set<OrigamiVideoCommentReaction>().AsNoTracking() where x.CommentId == ctx.Entity.Id select x.Id).ToList();
            hub.RowsAffected += db.PostCommentReactions.Where(x => reactions.Contains(x.Id)).ExecuteDelete();

            return hub;
        }

        public void SetComments(OrigamiVideo entity, long count) => this.Comments(entity, count);

        public Result<OrigamiVideoComment> Unpin(DataOperationContextFrontEnd<OrigamiVideoComment> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                var profile = db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
                if (profile.IsModerator)
                {
                    ctx.Entity.PinnedById = null;
                    return base.SmartUpdate(ctx, false);
                }
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage()) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            return new(ctx.Entity) { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public Task Update(IEnumerable<VideoCommentTotal> entities)
        {
            entities.Each(entity => this.SetComments(new(entity.VideoId), entity.TotalComments));
            return Task.CompletedTask;
        }
        public Result<OrigamiVideoComment> SmartUpdate(DataOperationContextFrontEnd<OrigamiVideoComment> ctx)
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

            return base.SmartUpdate(ctx, false);
        }

        public override Result<OrigamiVideoComment> UpdateValidation(DataOperationContext<OrigamiVideoComment> ctx)
        {
            return new Result<OrigamiVideoComment>(ctx.Entity, _validator);
        }
    }
}
