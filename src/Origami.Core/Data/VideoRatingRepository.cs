using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoRatingRepository :
        RepositoryOuterLayer<OrigamiVideoRating>,
        IVideoRatingRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoRatingRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            ISocialProfileRepository socialProfileRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _socialProfileRepository = socialProfileRepository;
        }

        public float CalculateRating(OrigamiVideo video)
        {
            var ratings = from x in ReadFromCache()
                          where x.VideoId == video.Id && x.Rating > 1
                          select x.Rating;

            return ratings.Any() ? (float)Math.Round((decimal)ratings.Sum(x => x) / ratings.Count(), 1) : 0;
        }

        public IEnumerable<OrigamiVideoRating> Ratings(OrigamiVideo Video)
        {
            return from x in ReadFromCache() where x.VideoId == Video.Id select x;
        }

        public IEnumerable<OrigamiVideoRating> RatingsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return from x in ReadFromCache() where x.SocialProfileId == socialProfile.Id select x;
        }

        public Result<OrigamiVideoRating> SmartCreate(DataOperationContextFrontEnd<OrigamiVideoRating> ctx)
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

        public Result<OrigamiVideoRating> SmartPurge(DataOperationContextFrontEnd<OrigamiVideoRating> ctx)
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
