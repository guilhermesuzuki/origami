using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PostCommentRepository :
        RepositoryOuterLayer<OrigamiPostComment>,
        IPostCommentRepository
    {
        protected readonly IValidator<OrigamiPostComment> _validator;
        protected readonly IPostCommentReactionRepository _postCommentReactionRepository;
        protected readonly ISocialProfileRepository _socialProfileRepository;
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostCommentRepository(
            IValidator<OrigamiPostComment> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IPostCommentReactionRepository postCommentReactionRepository,
            ISocialProfileRepository socialProfileRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _socialProfileRepository = socialProfileRepository;
            _postCommentReactionRepository = postCommentReactionRepository;
        }

        public override string DeletePermission => nameof(OrigamiRole.ModerateComments);
        public override string PurgePermission => nameof(OrigamiRole.PurgeComments);
        public override string ReadPermission => nameof(OrigamiRole.ViewComments);
        public override string RestorePermission => nameof(OrigamiRole.RestoreComments);

        public List<OrigamiPostComment> AllComments(OrigamiPost? entity)
        {
            if (entity != null)
            {
                //comments from a post
                var comments = ReadFromCache().NonDeleted().Where(x => x.PostId == entity.Id);
                return comments.OrderBy(x => x.PinnedById != null ? 0 : 1).ThenBy(x => x.DateCreated).ToList();
            }
            return [];
        }

        public List<OrigamiPostComment> CommentsFromProfile(OrigamiSocialProfile entity, bool deleted)
        {
            //comments from a post
            var comments = ReadFromCache().Where(x => x.SocialProfileId == entity.Id);
            //retrieves the deleted ones or not
            if (deleted == false) comments = comments.Where(x => x.IsDeleted == false);
            //returns ordered
            return comments.OrderBy(x => x.PinnedById != null ? 0 : 1).ThenByDescending(x => x.DateCreated).ToList();
        }

        public Result<OrigamiPostComment> SmartCreate(DataOperationContextFrontEnd<OrigamiPostComment> ctx)
        {
            try
            {
                _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
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
                return new(ctx.Entity) { ErrorMessage = Text.Original(Text.YouMadeTooManyCommentsIn5Minutes) };
            }

            return base.SmartCreate(ctx, false);
        }

        public override Result<OrigamiPostComment> CreateValidation(DataOperationContext<OrigamiPostComment> ctx)
        {
            return new Result<OrigamiPostComment>(ctx.Entity, _validator);
        }

        public Result<OrigamiPostComment> SmartDelete(DataOperationContextFrontEnd<OrigamiPostComment> ctx)
        {
            try
            {
                var profile = _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id)!;
                if (profile.IsModerator)
                {
                    return base.SmartDelete(ctx, false);
                }
            }
            catch (Exception)
            {
                return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            var comment = ReadFromCache().Id(ctx.Entity.Id);
            if (comment != null)
            {
                if (comment.SocialProfileId == ctx.SocialProfile.Id)
                {
                    return base.SmartDelete(ctx, false);
                }
            }

            return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public async Task<List<PostCommentTotal>> FastRead()
        {
            using var dbContextFactory = DbContextFactory.CreateDbContext();
            var sql = @"SELECT p.Id as PostId, (SELECT COUNT_BIG(1) FROM dbo.oi_PostComments pc WHERE pc.PostId = p.Id AND pc.IsDeleted = 0) as TotalComments FROM dbo.oi_Posts p";
            return await dbContextFactory.Database.SqlQueryRaw<PostCommentTotal>(sql).ToListAsync();
        }

        public long GetComments(OrigamiPost entity)
        {
            var key = entity.KeyForCachingComments();
            if (key.Has() == true)
            {
                var value = MemoryCache.Get(key);
                if (value is long l) return l;
            }
            return 0;
        }

        public Result<OrigamiPostComment> Pin(DataOperationContextFrontEnd<OrigamiPostComment> ctx)
        {
            try
            {
                var profile = _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id)!;
                if (profile.IsModerator)
                {
                    ctx.Entity.PinnedById = ctx.SocialProfile.Id;
                    return base.SmartUpdate(ctx, false);
                }
            }
            catch (Exception)
            {
                return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public override void PurgeRelationshipsFromCache(OrigamiPostComment entity)
        {
            base.PurgeRelationshipsFromCache(entity);
            var reactions = from x in _postCommentReactionRepository.ReadFromCache()
                            where x.CommentId == entity.Id
                            select x;

            reactions.Each(_postCommentReactionRepository.PurgeCache);
        }

        public override Result<OrigamiPostComment> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiPostComment> ctx)
        {
            var hub = base.PurgeRelationshipsFromDatabase(ctx);
            var reactions = (from x in _postCommentReactionRepository.ReadFromDatabase() where x.CommentId == ctx.Entity.Id select x.Id).ToList();
            using var db = DbContextFactory.CreateDbContext();
            hub.RowsAffected += db.PostCommentReactions.Where(x => reactions.Contains(x.Id)).ExecuteDelete();
            return hub;
        }

        public void SetComments(OrigamiPost entity, long count) => this.Comments(entity, count);
        public Result<OrigamiPostComment> Unpin(DataOperationContextFrontEnd<OrigamiPostComment> ctx)
        {
            try
            {
                var profile = _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id)!;
                if (profile.IsModerator)
                {
                    ctx.Entity.PinnedById = null;
                    return base.SmartUpdate(ctx, false);
                }
            }
            catch (Exception)
            {
                return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
            }

            return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }

        public Task Update(IEnumerable<PostCommentTotal> entities)
        {
            entities.Each(entity => this.Comments(new OrigamiPost(entity.PostId), entity.TotalComments));
            return Task.CompletedTask;
        }
        public Result<OrigamiPostComment> SmartUpdate(DataOperationContextFrontEnd<OrigamiPostComment> ctx)
        {
            try
            {
                _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            // user must have created the comment to edit it
            if (ctx.SocialProfile.Id != ctx.Entity.SocialProfileId)
            {
                return new(ctx.Entity) { ErrorMessage = Text.Original("You cannot edit this comment") };
            }

            var htmlValidation = this.HTMLValidation(ctx);
            if (htmlValidation.Ok == false) return htmlValidation;

            return base.SmartUpdate(ctx, false);
        }

        public override Result<OrigamiPostComment> UpdateValidation(DataOperationContext<OrigamiPostComment> ctx)
        {
            return new Result<OrigamiPostComment>(ctx.Entity, _validator);
        }
    }
}
