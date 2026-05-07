using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;
using System.Transactions;

namespace Origami.Core.Data
{
    public class SocialProfileDeleteRepository :
        RepositoryOuterLayer<OrigamiSocialProfileDelete>,
        ISocialProfileDeleteRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;
        protected readonly IContentCommentReactionRepository _contentCommentReactionRepository;
        protected readonly IContentCommentRepository _contentCommentRepository;
        protected readonly IContentRatingRepository _contentRatingRepository;
        protected readonly IContentReactionRepository _contentReactionRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SocialProfileDeleteRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            ISocialProfileRepository socialProfileRepository,
            IContentCommentReactionRepository contentCommentReactionRepository,
            IContentCommentRepository contentCommentRepository,
            IContentRatingRepository contentRatingRepository,
            IContentReactionRepository contentReactionRepository,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _socialProfileRepository = socialProfileRepository;
            _contentCommentReactionRepository = contentCommentReactionRepository;
            _contentCommentRepository = contentCommentRepository;
            _contentRatingRepository = contentRatingRepository;
            _contentReactionRepository = contentReactionRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.WipeDataOutFromSocialProfiles);

        public Result<OrigamiSocialProfileDelete> WipeDataOut(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var newContext = new DataOperationContext<OrigamiSocialProfileDelete>(ctx.User, new() { SocialProfileId = ctx.Entity.Id });
                var permission = CanCreate(newContext);
                if (permission.Ok == false) return permission;
            }

            using var db = DbContextFactory.CreateDbContext();

            var socialProfile = db.Set<OrigamiSocialProfile>().AsNoTracking().Where(x => x.Id == ctx.Entity.Id).FirstOrDefault();
            if (socialProfile != null)
            {
                var commentReactions = _contentCommentReactionRepository.ReactionsFromProfile(socialProfile).ToList();
                var comments = _contentCommentRepository.CommentsFromProfile(socialProfile, true);
                var ratings = _contentRatingRepository.RatingsFromProfile(socialProfile).ToList();

                var hub = new Result<OrigamiSocialProfileDelete>();

                using (var transaction = new TransactionScope())
                {
                    hub.OnSuccess(() => commentReactions.GetContexts(ctx).Call(_contentCommentReactionRepository.SmartPurge, false));
                    hub.OnSuccess(() => comments.Where(x => x.ParentId != null).GetContexts(ctx).Call(_contentCommentRepository.SmartPurge, false));
                    hub.OnSuccess(() => comments.Where(x => x.ParentId == null).GetContexts(ctx).Call(_contentCommentRepository.SmartPurge, false));
                    hub.OnSuccess(() => ratings.GetContexts(ctx).Call(_contentRatingRepository.SmartPurge, false));

                    hub.Entity = new()
                    {
                        Id = Guid.NewGuid(),
                        DateCreated = DateTime.UtcNow,
                        SocialProfileId = socialProfile.Id,
                        PostCommentReactions = commentReactions.Count,
                        PostComments = comments.Count,
                        PostRatings = ratings.Count,
                    };

                    var exists = db.Set<OrigamiSocialProfileDelete>().AsNoTracking()
                        .Where(x => x.DateCreated == hub.Entity.DateCreated)
                        .Where(x => x.SocialProfileId == hub.Entity.SocialProfileId)
                        .Any() ? 1 : 0;

                    var deleteContext = new DataOperationContext<OrigamiSocialProfileDelete>(ctx.User, ctx.DateTime, hub.Entity);

                    if (exists == 1) hub.OnSuccess(() => SmartUpdate(deleteContext, false).Push(hub));
                    if (exists == 0) hub.OnSuccess(() => SmartCreate(deleteContext, false).Push(hub));

                    //exits the method, in case of a failure
                    if (hub.Ok == false) return hub;

                    //commits the transaction
                    transaction.Complete();
                }

                //needs to update cache
                commentReactions.ForEach(_contentCommentReactionRepository.PurgeCache);
                comments.ForEach(_contentCommentRepository.PurgeCache);
                ratings.ForEach(_contentRatingRepository.PurgeCache);

                return hub;
            }

            return new() { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }
    }
}
