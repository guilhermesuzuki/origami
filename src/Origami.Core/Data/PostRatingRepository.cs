using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PostRatingRepository :
        RepositoryOuterLayer<OrigamiPostRating>,
        IPostRatingRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostRatingRepository(
            Text text,
            IMemoryCache memoryCache,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            ISocialProfileRepository socialProfileRepository,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _socialProfileRepository = socialProfileRepository;
        }

        public float CalculateRating(OrigamiPost post)
        {
            var ratings = from x in ReadFromCache()
                          where x.PostId == post.Id && x.Rating > 1
                          select x.Rating;

            return ratings.Any() ? (float)Math.Round((decimal)ratings.Sum(x => x) / ratings.Count(), 1) : 0;
        }

        public IEnumerable<OrigamiPostRating> Ratings(OrigamiPost post)
        {
            return from x in ReadFromCache() where x.PostId == post.Id select x;
        }

        public IEnumerable<OrigamiPostRating> RatingsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return from x in ReadFromCache() where x.SocialProfileId == socialProfile.Id select x;
        }

        public Result<OrigamiPostRating> SmartCreate(DataOperationContextFrontEnd<OrigamiPostRating> ctx)
        {
            try
            {
                _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            return base.SmartCreate(ctx, false);
        }

        public Result<OrigamiPostRating> SmartPurge(DataOperationContextFrontEnd<OrigamiPostRating> ctx)
        {
            try
            {
                _socialProfileRepository.ReadFromDatabase().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            return base.SmartPurge(ctx, false);
        }
    }
}
