using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class ContentReactionRepository :
        RepositoryOuterLayer<OrigamiContentReaction>,
        IContentReactionRepository
    {
        protected readonly IEventRepository _eventRepository;
        protected readonly IValidator<OrigamiContentReaction> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentReactionRepository(
            IEventRepository eventRepository,
            IValidator<OrigamiContentReaction> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _eventRepository = eventRepository;
        }

        public IEnumerable<OrigamiContentReaction> Reactions(OrigamiContent entity)
        {
            return ReadFromCache().Where(x => x.ContentId == entity.Id);
        }

        public IEnumerable<OrigamiContentReaction> ReactionsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return ReadFromCache().Where(x => x.SocialProfileId == socialProfile.Id);
        }

        public Result<OrigamiContentReaction> SmartCreate(DataOperationContextFrontEnd<OrigamiContentReaction> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();

            try
            {
                db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            var hasReacted = db.Set<OrigamiContentReaction>()
                .AsNoTracking()
                .Where(x => x.ContentId == ctx.Entity.ContentId)
                .Where(x => x.SocialProfileId == ctx.SocialProfile.Id)
                .Where(x => x.Reaction == ctx.Entity.Reaction)
                .Any();

            if (hasReacted)
            {
                return new(ctx.Entity, Text.Original("You already reacted with this emoji"));
            }

            var hub = base.SmartCreate(ctx, false);

            _eventRepository.SocialProfileReactsToContent(ctx.SocialProfile.Id, ctx.Entity.Id);

            return hub;
        }

        public Result<OrigamiContentReaction> SmartPurge(DataOperationContextFrontEnd<OrigamiContentReaction> ctx)
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

        public override Result<OrigamiContentReaction> CreateValidation(DataOperationContext<OrigamiContentReaction> ctx)
        {
            var hasReacted = ReadFromCache()
                .Where(x => x.ContentId == ctx.Entity.ContentId)
                .Where(x => x.SocialProfileId == ctx.Entity.SocialProfileId)
                .Where(x => x.Reaction == ctx.Entity.Reaction)
                .Any();

            if (hasReacted)
            {
                return new(ctx.Entity, Text.Original("You already reacted with this emoji"));
            }

            return new(ctx.Entity);
        }
    }
}
