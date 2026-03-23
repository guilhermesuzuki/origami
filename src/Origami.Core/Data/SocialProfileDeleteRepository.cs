using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System.Transactions;

namespace Origami.Core.Data
{
    public class SocialProfileDeleteRepository :
        RepositoryOuterLayer<OrigamiSocialProfileDelete>,
        ISocialProfileDeleteRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SocialProfileDeleteRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            ISocialProfileRepository socialProfileRepository,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _socialProfileRepository = socialProfileRepository;
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
                var postCommentReactions = _postCommentReactionRepository.ReactionsFromProfile(socialProfile).ToList();
                var postComments = _postCommentRepository.CommentsFromProfile(socialProfile, true);
                var postRatings = _postRatingRepository.RatingsFromProfile(socialProfile).ToList();
                var videoCommentReactions = _videoCommentReactionRepository.ReactionsFromProfile(socialProfile).ToList();
                var videoComments = _videoCommentRepository.CommentsFromProfile(socialProfile, true);
                var videoRatings = _videoRatingRepository.RatingsFromProfile(socialProfile).ToList();

                var hub = new Result<OrigamiSocialProfileDelete>();

                using (var transaction = new TransactionScope())
                {
                    hub.OnSuccess(() => postCommentReactions.GetContexts(ctx).Call(_postCommentReactionRepository.SmartPurge, false));
                    hub.OnSuccess(() => postComments.Where(x => x.ParentId != null).GetContexts(ctx).Call(_postCommentRepository.SmartPurge, false));
                    hub.OnSuccess(() => postComments.Where(x => x.ParentId == null).GetContexts(ctx).Call(_postCommentRepository.SmartPurge, false));
                    hub.OnSuccess(() => postRatings.GetContexts(ctx).Call(_postRatingRepository.SmartPurge, false));
                    hub.OnSuccess(() => videoCommentReactions.GetContexts(ctx).Call(_videoCommentReactionRepository.SmartPurge, false));
                    hub.OnSuccess(() => videoComments.Where(x => x.ParentId != null).GetContexts(ctx).Call(_videoCommentRepository.SmartPurge, false));
                    hub.OnSuccess(() => videoComments.Where(x => x.ParentId == null).GetContexts(ctx).Call(_videoCommentRepository.SmartPurge, false));
                    hub.OnSuccess(() => videoRatings.GetContexts(ctx).Call(_videoRatingRepository.SmartPurge, false));

                    hub.Entity = new()
                    {
                        Id = Guid.NewGuid(),
                        DateCreated = DateTime.UtcNow,
                        SocialProfileId = socialProfile.Id,
                        PostCommentReactions = postCommentReactions.Count,
                        PostComments = postComments.Count,
                        PostRatings = postRatings.Count,
                        VideoCommentReactions = videoCommentReactions.Count,
                        VideoComments = videoComments.Count,
                        VideoRatings = videoRatings.Count,
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
                postCommentReactions.ForEach(x => _postCommentReactionRepository.PurgeCache(x));
                postComments.ForEach(x => _postCommentRepository.PurgeCache(x));
                postRatings.ForEach(x => _postRatingRepository.PurgeCache(x));

                videoCommentReactions.ForEach(x => _videoCommentReactionRepository.PurgeCache(x));
                videoComments.ForEach(x => _videoCommentRepository.PurgeCache(x));
                videoRatings.ForEach(x => _videoRatingRepository.PurgeCache(x));

                return hub;
            }

            return new() { Error = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
        }
    }
}
