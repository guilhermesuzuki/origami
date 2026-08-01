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

            var socialProfile = TestFacebookProfile.Clone();

            socialProfile.Email = "invalid-email";

            var hub = superRepository.SocialProfiles.SmartSave(socialProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(false);
            hub.Messages.Count.ShouldBe(1);
            hub.Messages[0].Message.ShouldBe("When provided, email must be valid");
            hub.Messages[0].MessageType.ShouldBe(Core.Models.ResultMessage.MessageTypes.Error);

            var query = from a in db.SocialProfiles where a.Id == TestFacebookProfile.Id select a;
            var dbSocialProfile = query.FirstOrDefault();
            dbSocialProfile.ShouldBeNull();

            var cacheSocialProfile = superRepository.SocialProfiles.ReadFromCache().Id(TestFacebookProfile.Id);
            cacheSocialProfile.ShouldBeNull();
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

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var hub = superRepository.SocialProfiles.SmartSave(TestFacebookProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(true);

            var query = from a in db.SocialProfiles where a.Id == TestFacebookProfile.Id select a;
            var socialProfile = query.Single();
            socialProfile.ShouldNotBeNull();
            socialProfile.SocialNetwork.ShouldBe(TestFacebookProfile.SocialNetwork);
            socialProfile.UserId.ShouldBe(TestFacebookProfile.UserId);
            socialProfile.Email.ShouldBe(TestFacebookProfile.Email);
            socialProfile.EmailFromSocialNetwork.ShouldBe(TestFacebookProfile.EmailFromSocialNetwork);
            socialProfile.Name.ShouldBe(TestFacebookProfile.Name);

            var cacheSocialProfile = superRepository.SocialProfiles.ReadFromCache().Id(TestFacebookProfile.Id);
            cacheSocialProfile.ShouldNotBeNull();
            cacheSocialProfile.SocialNetwork.ShouldBe(TestFacebookProfile.SocialNetwork);
            cacheSocialProfile.UserId.ShouldBe(TestFacebookProfile.UserId);
            cacheSocialProfile.Email.ShouldBe(TestFacebookProfile.Email);
            cacheSocialProfile.EmailFromSocialNetwork.ShouldBe(TestFacebookProfile.EmailFromSocialNetwork);
            cacheSocialProfile.Name.ShouldBe(TestFacebookProfile.Name);
        }

        [Theory]
        [InlineData(true)]
        public void Insert_WhenEntityIsBlocked_ShouldPersistRecord(bool useTransaction)
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = useTransaction ? new TransactionScope() : null;
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var hub = superRepository.SocialProfiles.SmartSave(TestFacebookProfileButUserIsBlocked.GetContext(TestUser), false);

            hub.Ok.ShouldBe(true);

            var query = from a in db.SocialProfiles where a.Id == TestFacebookProfileButUserIsBlocked.Id select a;
            var socialProfile = query.Single();
            socialProfile.ShouldNotBeNull();
            socialProfile.SocialNetwork.ShouldBe(TestFacebookProfileButUserIsBlocked.SocialNetwork);
            socialProfile.UserId.ShouldBe(TestFacebookProfileButUserIsBlocked.UserId);
            socialProfile.Email.ShouldBe(TestFacebookProfileButUserIsBlocked.Email);
            socialProfile.EmailFromSocialNetwork.ShouldBe(TestFacebookProfileButUserIsBlocked.EmailFromSocialNetwork);
            socialProfile.Name.ShouldBe(TestFacebookProfileButUserIsBlocked.Name);

            var cacheSocialProfile = superRepository.SocialProfiles.ReadFromCache().Id(TestFacebookProfileButUserIsBlocked.Id);
            cacheSocialProfile.ShouldNotBeNull();
            cacheSocialProfile.SocialNetwork.ShouldBe(TestFacebookProfileButUserIsBlocked.SocialNetwork);
            cacheSocialProfile.UserId.ShouldBe(TestFacebookProfileButUserIsBlocked.UserId);
            cacheSocialProfile.Email.ShouldBe(TestFacebookProfileButUserIsBlocked.Email);
            cacheSocialProfile.EmailFromSocialNetwork.ShouldBe(TestFacebookProfileButUserIsBlocked.EmailFromSocialNetwork);
            cacheSocialProfile.Name.ShouldBe(TestFacebookProfileButUserIsBlocked.Name);
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

            var socialProfile = TestFacebookProfile.Clone();

            socialProfile.Name = string.Empty;
            socialProfile.FirstName = string.Empty;
            socialProfile.LastName = string.Empty;

            var hub = superRepository.SocialProfiles.SmartSave(socialProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(false);
            hub.Messages.Count.ShouldBe(1);
            hub.Messages[0].Message.ShouldBe("Name is required");
            hub.Messages[0].MessageType.ShouldBe(Core.Models.ResultMessage.MessageTypes.Error);

            var query = from a in db.SocialProfiles where a.Id == TestFacebookProfile.Id select a;
            var dbSocialProfile = query.FirstOrDefault();
            dbSocialProfile.ShouldBeNull();

            var cacheSocialProfile = superRepository.SocialProfiles.ReadFromCache().Id(TestFacebookProfile.Id);
            cacheSocialProfile.ShouldBeNull();
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

            var socialProfile = TestFacebookProfile.Clone();

            socialProfile.ProfilePage = "invalid-profile-page";

            var hub = superRepository.SocialProfiles.SmartSave(socialProfile.GetContext(TestUser), false);

            hub.Ok.ShouldBe(false);
            hub.Messages.Count.ShouldBe(1);
            hub.Messages[0].Message.ShouldBe("Profile page url: URL must be a valid website address");
            hub.Messages[0].MessageType.ShouldBe(Core.Models.ResultMessage.MessageTypes.Error);

            var query = from a in db.SocialProfiles where a.Id == TestFacebookProfile.Id select a;
            var dbSocialProfile = query.FirstOrDefault();
            dbSocialProfile.ShouldBeNull();

            var cacheSocialProfile = superRepository.SocialProfiles.ReadFromCache().Id(TestFacebookProfile.Id);
            cacheSocialProfile.ShouldBeNull();
        }
    }
}
