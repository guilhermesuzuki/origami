using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class ContentRatingRepository :
        RepositoryOuterLayer<OrigamiContentRating>,
        IContentRatingRepository
    {
        protected readonly ISocialProfileRepository _socialProfileRepository;
        protected readonly IValidator<OrigamiContentRating> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentRatingRepository(
            IAppFacade appFacade,
            IValidator<OrigamiContentRating> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            ISocialProfileRepository socialProfileRepository,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _validator = validator;
            _socialProfileRepository = socialProfileRepository;
        }

        public float CalculateRating(OrigamiContent content)
        {
            var ratings = from x in ReadFromCache() where x.ContentId == content.Id && x.Rating > 1 select x.Rating;
            return ratings.Any() ? (float)Math.Round((decimal)ratings.Sum(x => x) / ratings.Count(), 1) : 0;
        }

        public IEnumerable<OrigamiContentRating> Ratings(OrigamiContent content)
        {
            return from x in ReadFromCache() where x.ContentId == content.Id select x;
        }

        public IEnumerable<OrigamiContentRating> RatingsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return from x in ReadFromCache() where x.SocialProfileId == socialProfile.Id select x;
        }

        public Result<OrigamiContentRating> SmartCreate(DataOperationContextFrontEnd<OrigamiContentRating> ctx)
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

        public Result<OrigamiContentRating> SmartPurge(DataOperationContextFrontEnd<OrigamiContentRating> ctx)
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
    }
}
