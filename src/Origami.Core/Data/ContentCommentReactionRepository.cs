using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class ContentCommentReactionRepository :
        RepositoryOuterLayer<OrigamiContentCommentReaction>,
        IContentCommentReactionRepository
    {
        protected readonly IEventRepository _eventRepository;
        protected readonly IValidator<OrigamiContentCommentReaction> _validator;

        public override string CreatePermission => nameof(OrigamiRole.ModerateComments);
        public override string DeletePermission => nameof(OrigamiRole.ModerateComments);
        public override string UpdatePermission => nameof(OrigamiRole.ModerateComments);
        public override string PurgePermission => nameof(OrigamiRole.ModerateComments);

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentCommentReactionRepository(
            IAppFacade appFacade,
            IEventRepository eventRepository,
            IValidator<OrigamiContentCommentReaction> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _validator = validator;
            _eventRepository = eventRepository;
        }

        public IEnumerable<OrigamiContentCommentReaction> Reactions(OrigamiContentComment entity)
        {
            return ReadFromCache().Where(x => x.CommentId == entity.Id);
        }

        public IEnumerable<OrigamiContentCommentReaction> ReactionsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return ReadFromCache().Where(x => x.SocialProfileId == socialProfile.Id);
        }

        public Result<OrigamiContentCommentReaction> SmartCreate(DataOperationContextFrontEnd<OrigamiContentCommentReaction> ctx)
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

            var hub = base.SmartCreate(ctx, false);

            if (hub.Ok == true)
            {
                _eventRepository.SocialProfileReactsToComment(ctx.SocialProfile, ctx.Entity);
            }

            return hub;
        }

        public Result<OrigamiContentCommentReaction> SmartPurge(DataOperationContextFrontEnd<OrigamiContentCommentReaction> ctx)
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

            var hub = base.SmartPurge(ctx, false);

            if (hub.Ok == true)
            {
                _eventRepository.SocialProfileCancelsReactionToComment(ctx.SocialProfile, ctx.Entity);
            }

            return hub;
        }
    }
}
