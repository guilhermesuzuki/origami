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

        public Result<SocialProfileDeletesCommentEvent> SocialProfileDeletesComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment)
        {
            var @event = new SocialProfileDeletesCommentEvent
            {
                SocialProfileId = socialProfile.Id,
                CommentId = comment.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileDeletesCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileEditsCommentEvent> SocialProfileEditsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment)
        {
            var @event = new SocialProfileEditsCommentEvent
            {
                SocialProfileId = socialProfile.Id,
                CommentId = comment.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileEditsCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileLogsIntoWebsiteEvent> SocialProfileLogsIntoWebsite(OrigamiSocialProfile socialProfile)
        {
            var @event = new SocialProfileLogsIntoWebsiteEvent
            {
                SocialProfileId = socialProfile.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileLogsIntoWebsiteEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfilePinsCommentEvent> SocialProfilePinsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment)
        {
            var @event = new SocialProfilePinsCommentEvent
            {
                SocialProfileId = socialProfile.Id,
                CommentId = comment.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfilePinsCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileReactsToCommentEvent> SocialProfileReactsToComment(OrigamiSocialProfile socialProfile, OrigamiContentReaction reaction)
        {
            var @event = new SocialProfileReactsToCommentEvent
            {
                SocialProfileId = socialProfile.Id,
                ReactionId = reaction.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileReactsToCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileReactsToContentEvent> SocialProfileReactsToContent(OrigamiSocialProfile socialProfile, OrigamiContentReaction reaction)
        {
            var @event = new SocialProfileReactsToContentEvent
            {
                SocialProfileId = socialProfile.Id,
                ReactionId = reaction.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileReactsToContentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileRepliesToCommentEvent> SocialProfileRepliesToComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment)
        {
            var @event = new SocialProfileRepliesToCommentEvent
            {
                SocialProfileId = socialProfile.Id,
                CommentId = comment.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileRepliesToCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileRepliesToContentEvent> SocialProfileRepliesToContent(OrigamiSocialProfile socialProfile, OrigamiContentComment comment)
        {
            var @event = new SocialProfileRepliesToContentEvent
            {
                SocialProfileId = socialProfile.Id,
                ContentId = comment.ContentId,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileRepliesToContentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileSubscribesToWebsiteEvent> SocialProfileSubscribesToWebsite(OrigamiSocialProfile socialProfile)
        {
            var @event = new SocialProfileSubscribesToWebsiteEvent
            {
                SocialProfileId = socialProfile.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileSubscribesToWebsiteEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileUnpinsCommentEvent> SocialProfileUnpinsComment(OrigamiSocialProfile socialProfile, OrigamiContentComment comment)
        {
            var @event = new SocialProfileUnpinsCommentEvent
            {
                SocialProfileId = socialProfile.Id,
                CommentId = comment.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileUnpinsCommentEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }

        public Result<SocialProfileUnsubscribesFromWebsiteEvent> SocialProfileUnsubscribesFromWebsite(OrigamiSocialProfile socialProfile)
        {
            var @event = new SocialProfileUnsubscribesFromWebsiteEvent
            {
                SocialProfileId = socialProfile.Id,
                DateCreated = DateTime.UtcNow,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.SocialProfileUnsubscribesFromWebsiteEvents.Add(@event);
            db.SaveChanges();

            return new(@event);
        }
    }
}
