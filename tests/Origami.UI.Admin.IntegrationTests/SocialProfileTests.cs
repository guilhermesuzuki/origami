using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class SocialProfileTests : CustomClassFixture
    {
        protected readonly HubContentPostTests _postTests;

        public SocialProfileTests() : base()
        {
            _postTests = new();
        }

        [Fact]
        public void Insert_WhenEmailIsInvalid_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var socialProfile = this.TestFacebookProfile.Clone();

            socialProfile.Email = "invalid-email";

            var hub = superRepository.SocialProfiles.SmartSave(socialProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(false);
            hub.Messages.Count.ShouldBe(1);
            hub.Messages[0].Message.ShouldBe("When provided, email must be valid");
            hub.Messages[0].MessageType.ShouldBe(Core.Models.ResultMessage.MessageTypes.Error);
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var hub = superRepository.SocialProfiles.SmartSave(this.TestFacebookProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(true);

            var query = from a in db.SocialProfiles where a.Id == this.TestFacebookProfile.Id select a;

            var socialProfile = query.Single();
            socialProfile.ShouldNotBeNull();
            socialProfile.SocialNetwork.ShouldBe(this.TestFacebookProfile.SocialNetwork);
            socialProfile.UserId.ShouldBe(this.TestFacebookProfile.UserId);
            socialProfile.Email.ShouldBe(this.TestFacebookProfile.Email);
            socialProfile.EmailFromSocialNetwork.ShouldBe(this.TestFacebookProfile.EmailFromSocialNetwork);
            socialProfile.Name.ShouldBe(this.TestFacebookProfile.Name);
        }

        [Fact]
        public void Insert_WhenNameIsInvalid_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var socialProfile = this.TestFacebookProfile.Clone();

            socialProfile.Name = string.Empty;
            socialProfile.FirstName = string.Empty;
            socialProfile.LastName = string.Empty;

            var hub = superRepository.SocialProfiles.SmartSave(socialProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(false);
            hub.Messages.Count.ShouldBe(1);
            hub.Messages[0].Message.ShouldBe("Name is required");
            hub.Messages[0].MessageType.ShouldBe(Core.Models.ResultMessage.MessageTypes.Error);
        }

        [Fact]
        public void Insert_WhenProfilePageIsInvalid_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var socialProfile = this.TestFacebookProfile.Clone();

            socialProfile.ProfilePage = "invalid-profile-page";

            var hub = superRepository.SocialProfiles.SmartSave(socialProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(false);
            hub.Messages.Count.ShouldBe(1);
            hub.Messages[0].Message.ShouldBe("Profile page url: URL must be a valid website address");
            hub.Messages[0].MessageType.ShouldBe(Core.Models.ResultMessage.MessageTypes.Error);
        }
    }
}
