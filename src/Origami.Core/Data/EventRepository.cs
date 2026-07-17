using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;
using Origami.Core.Models.Events;

namespace Origami.Core.Data
{
    public class EventRepository : RepositoryOuterLayer<OrigamiEvent>, IEventRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public EventRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            
        }

        public Result<SocialProfileDeletesCommentEvent> SocialProfileDeletesComment(Guid socialProfile, Guid comment)
        {
            var @event = new SocialProfileDeletesCommentEvent
            {
                SocialProfileId = socialProfile,
                CommentId = comment,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileDeletesCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileEditsCommentEvent> SocialProfileEditsComment(Guid socialProfile, Guid comment)
        {
            var @event = new SocialProfileEditsCommentEvent
            {
                SocialProfileId = socialProfile,
                CommentId = comment,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileEditsCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileLogsIntoWebsiteEvent> SocialProfileLogsIntoWebsite(Guid socialProfile)
        {
            var @event = new SocialProfileLogsIntoWebsiteEvent
            {
                SocialProfileId = socialProfile,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileLogsIntoWebsiteEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileReactsToCommentEvent> SocialProfileReactsToComment(Guid socialProfile, Guid reaction)
        {
            var @event = new SocialProfileReactsToCommentEvent
            {
                SocialProfileId = socialProfile,
                ReactionId = reaction,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileReactsToCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileReactsToContentEvent> SocialProfileReactsToContent(Guid socialProfile, Guid reaction)
        {
            var @event = new SocialProfileReactsToContentEvent
            {
                SocialProfileId = socialProfile,
                ReactionId = reaction,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileReactsToContentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileRepliesToCommentEvent> SocialProfileRepliesToComment(Guid socialProfile, Guid comment)
        {
            var @event = new SocialProfileRepliesToCommentEvent
            {
                SocialProfileId = socialProfile,
                CommentId = comment,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileRepliesToCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileRepliesToContentEvent> SocialProfileRepliesToContent(Guid socialProfile, Guid content)
        {
            var @event = new SocialProfileRepliesToContentEvent
            {
                SocialProfileId = socialProfile,
                ContentId = content,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileRepliesToContentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileSubscribesToWebsiteEvent> SocialProfileSubscribesToWebsite(Guid socialProfile)
        {
            var @event = new SocialProfileSubscribesToWebsiteEvent
            {
                SocialProfileId = socialProfile,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileSubscribesToWebsiteEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileUnsubscribesFromWebsiteEvent> SocialProfileUnsubscribesFromWebsite(Guid socialProfile)
        {
            var @event = new SocialProfileUnsubscribesFromWebsiteEvent
            {
                SocialProfileId = socialProfile,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileUnsubscribesFromWebsiteEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }
    }
}
