using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class ContentCommentReactionTests : CustomClassFixture
    {
        protected readonly ContentCommentTests _contentCommentTests = new();

        public ContentCommentReactionTests()
        {
            
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _contentCommentTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);

            var reaction = new OrigamiContentCommentReaction
            {
                CommentId = Comment.Id,
                DateCreated = DateTime.UtcNow,
                IsBot = false,
                IsMobileDevice = false,
                SocialProfileId = TestFacebookId,
                Reaction = "❤️",
            };

            var hub = superRepository.ContentCommentReactions.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, reaction));
            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            var query = from a in db.ContentCommentReactions.AsNoTracking() where a.Id == reaction.Id select a;
            var dbReaction = query.Single();
            dbReaction.CommentId.ShouldBe(reaction.CommentId);
            dbReaction.DateCreated.ShouldBe(reaction.DateCreated);
            dbReaction.IsBot.ShouldBe(reaction.IsBot);
            dbReaction.IsMobileDevice.ShouldBe(reaction.IsMobileDevice);
            dbReaction.SocialProfileId.ShouldBe(reaction.SocialProfileId);
            dbReaction.Reaction.ShouldBe(reaction.Reaction);

            var query2 = from a in db.Events.AsNoTracking().OfType<SocialProfileReactsToCommentEvent>() 
                         where a.ReactionId == reaction.Id
                         where a.SocialProfileId == reaction.SocialProfileId
                         select a;
            var dbEvent = query2.Single();
        }

        [Fact]
        public void Purge_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _contentCommentTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);

            var reaction = new OrigamiContentCommentReaction
            {
                CommentId = Comment.Id,
                DateCreated = DateTime.UtcNow,
                IsBot = false,
                IsMobileDevice = false,
                SocialProfileId = TestFacebookId,
                Reaction = "❤️",
            };

            superRepository.ContentCommentReactions.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, reaction));

            // private scope
            {
                var query = from a in db.ContentCommentReactions.AsNoTracking() where a.Id == reaction.Id select a;
                var dbReaction = query.Single();
                dbReaction.CommentId.ShouldBe(reaction.CommentId);
                dbReaction.DateCreated.ShouldBe(reaction.DateCreated);
                dbReaction.IsBot.ShouldBe(reaction.IsBot);
                dbReaction.IsMobileDevice.ShouldBe(reaction.IsMobileDevice);
                dbReaction.SocialProfileId.ShouldBe(reaction.SocialProfileId);
                dbReaction.Reaction.ShouldBe(reaction.Reaction);
            }

            //private scope
            {
                var query = from a in db.Events.AsNoTracking().OfType<SocialProfileReactsToCommentEvent>()
                             where a.ReactionId == reaction.Id
                             where a.SocialProfileId == reaction.SocialProfileId
                             select a;
                var dbEvent = query.Single();
            }
            

            superRepository.ContentCommentReactions.SmartPurge(new(TestFacebookProfile, DateTime.UtcNow, reaction));

            //private scope
            {
                var query = from a in db.Events.AsNoTracking().OfType<SocialProfileCancelsReactionToCommentEvent>()
                            where a.ReactionId == reaction.Id
                            where a.SocialProfileId == reaction.SocialProfileId
                            select a;
                var dbEvent = query.Single();
            }
        }
    }
}
