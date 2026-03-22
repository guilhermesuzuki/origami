using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PostCommentReactionRepository :
        RepositoryOuterLayer<OrigamiPostCommentReaction>,
        IPostCommentReactionRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostCommentReactionRepository(
            Text text,
            IMemoryCache memoryCache,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            ISocialProfileRepository socialProfileRepository,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _socialProfileRepository = socialProfileRepository;
        }

        public Result<OrigamiPostCommentReaction> SmartCreate(DataOperationContextFrontEnd<OrigamiPostCommentReaction> ctx)
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

            return base.SmartCreate(ctx, false);
        }

        public Result<OrigamiPostCommentReaction> SmartPurge(DataOperationContextFrontEnd<OrigamiPostCommentReaction> ctx)
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

            return base.SmartPurge(ctx, false);
        }

        public IEnumerable<OrigamiPostCommentReaction> Reactions(OrigamiPostComment entity)
        {
            return ReadFromCache().Where(x => x.CommentId == entity.Id);
        }

        public IEnumerable<OrigamiPostCommentReaction> ReactionsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return ReadFromCache().Where(x => x.SocialProfileId == socialProfile.Id);
        }
    }
}
