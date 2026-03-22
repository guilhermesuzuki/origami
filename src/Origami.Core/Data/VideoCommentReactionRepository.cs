using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoCommentReactionRepository :
        RepositoryOuterLayer<OrigamiVideoCommentReaction>,
        IVideoCommentReactionRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoCommentReactionRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            ISocialProfileRepository socialProfileRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _socialProfileRepository = socialProfileRepository;
        }

        public Result<OrigamiVideoCommentReaction> SmartCreate(DataOperationContextFrontEnd<OrigamiVideoCommentReaction> ctx)
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

        public Result<OrigamiVideoCommentReaction> SmartPurge(DataOperationContextFrontEnd<OrigamiVideoCommentReaction> ctx)
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

        public IEnumerable<OrigamiVideoCommentReaction> Reactions(OrigamiVideoComment entity)
        {
            return ReadFromCache().Where(x => x.CommentId == entity.Id);
        }

        public IEnumerable<OrigamiVideoCommentReaction> ReactionsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return ReadFromCache().Where(x => x.SocialProfileId == socialProfile.Id);
        }
    }
}
