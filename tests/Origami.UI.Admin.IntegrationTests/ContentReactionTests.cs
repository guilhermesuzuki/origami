using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.Events;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class ContentReactionTests : CustomClassFixture
    {
        protected readonly HubContentPostTests _hubContentPostTests = new();
        protected readonly SocialProfileTests _socialProfileTests = new();

        public ContentReactionTests() : base()
        {
            
        }

        [Theory]
        [InlineData(true)]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord(bool useTransaction)
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = useTransaction ? new TransactionScope() : null;
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _hubContentPostTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);
            _socialProfileTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);

            var reaction = new OrigamiContentReaction
            {
                ContentId = ContentId,
                DateCreated = DateTime.UtcNow,
                IsBot = false,
                IsMobileDevice = false,
                SocialProfileId = TestFacebookId,
                Reaction = "❤️",
            };

            var hub = superRepository.ContentReactions.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, reaction));
            hub.Ok.ShouldBeTrue();

            var query = from a in db.ContentReactions.AsNoTracking() where a.Id == reaction.Id select a;
            var dbReaction = query.Single();
            dbReaction.ContentId.ShouldBe(reaction.ContentId);
            dbReaction.DateCreated.ShouldBe(reaction.DateCreated);
            dbReaction.IsBot.ShouldBe(reaction.IsBot);
            dbReaction.IsMobileDevice.ShouldBe(reaction.IsMobileDevice);
            dbReaction.SocialProfileId.ShouldBe(reaction.SocialProfileId);
            dbReaction.Reaction.ShouldBe(reaction.Reaction);

            var query2 = from a in db.SocialProfileReactsToContentEvents.AsNoTracking()
                         where a.ReactionId == reaction.Id
                         where a.SocialProfileId == reaction.SocialProfileId
                         select a;
            var dbEvent = query2.Single();
        }

        [Theory]
        [InlineData(true)]
        public void Purge_WhenEntityIsValid_ShouldPersistRecord(bool useTransaction)
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = useTransaction ? new TransactionScope() : null;
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _hubContentPostTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);
            _socialProfileTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);

            var reaction = new OrigamiContentReaction
            {
                ContentId = ContentId,
                DateCreated = DateTime.UtcNow,
                IsBot = false,
                IsMobileDevice = false,
                SocialProfileId = TestFacebookId,
                Reaction = "❤️",
            };

            //private scope
            {
                var hub = superRepository.ContentReactions.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, reaction));
                hub.Ok.ShouldBeTrue();
            }

            //private scope
            {
                var query = from a in db.ContentReactions.AsNoTracking() where a.Id == reaction.Id select a;
                var dbReaction = query.Single();
                dbReaction.ContentId.ShouldBe(reaction.ContentId);
                dbReaction.DateCreated.ShouldBe(reaction.DateCreated);
                dbReaction.IsBot.ShouldBe(reaction.IsBot);
                dbReaction.IsMobileDevice.ShouldBe(reaction.IsMobileDevice);
                dbReaction.SocialProfileId.ShouldBe(reaction.SocialProfileId);
                dbReaction.Reaction.ShouldBe(reaction.Reaction);
            }

            //private scope
            {
                var query = from a in db.SocialProfileReactsToContentEvents.AsNoTracking()
                             where a.ReactionId == reaction.Id
                             where a.SocialProfileId == reaction.SocialProfileId
                             select a;
                var dbEvent = query.Single();
            }

            //private scope
            {
                var hub = superRepository.ContentReactions.SmartPurge(new(TestFacebookProfile, DateTime.UtcNow, reaction));
                hub.Ok.ShouldBeTrue();
            }

            //private scope
            {
                var query = from a in db.Events.AsNoTracking().OfType<SocialProfileCancelsReactionToContentEvent>()
                            where a.ReactionId == reaction.Id
                            where a.SocialProfileId == reaction.SocialProfileId
                            select a;
                var dbEvent = query.Single();
            }
        }
    }
}
