using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class ContentCommentTests : CustomClassFixture
    {
        protected readonly HubContentPostTests _hubContentPostTests = new();
        protected readonly SocialProfileTests _socialProfileTests = new();

        public ContentCommentTests() : base()
        {
            
        }

        [Theory]
        [InlineData(true)]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord(bool useTransaction)
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = useTransaction ? new TransactionScope() : null;
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _hubContentPostTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);
            _socialProfileTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);

            superRepository.ContentComments.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, Comment));

            //private scope
            {
                var query = from a in db.ContentComments.AsNoTracking() where a.Id == Comment.Id select a;
                var dbComment = query.Single();
                dbComment.Content.ShouldBe(Comment.Content);
                dbComment.ContentId.ShouldBe(Comment.ContentId);
                dbComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);

                var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
                cacheComment.ShouldNotBeNull();
                cacheComment.Content.ShouldBe(Comment.Content);
                cacheComment.ContentId.ShouldBe(Comment.ContentId);
                cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
            }

            superRepository.ContentComments.SmartDelete(new(TestFacebookProfile, DateTime.UtcNow, Comment));

            //private scope
            {
                var query = from a in db.ContentComments.AsNoTracking() where a.Id == Comment.Id select a;
                var dbComment = query.Single();
                dbComment.Content.ShouldBe(Comment.Content);
                dbComment.ContentId.ShouldBe(Comment.ContentId);
                dbComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
                dbComment.IsDeleted.ShouldBeTrue();

                var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
                cacheComment.ShouldNotBeNull();
                cacheComment.Content.ShouldBe(Comment.Content);
                cacheComment.ContentId.ShouldBe(Comment.ContentId);
                cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
                cacheComment.IsDeleted.ShouldBeTrue();
            }
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

            superRepository.ContentComments.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, Comment));

            var query = from a in db.ContentComments.AsNoTracking() where a.Id == Comment.Id select a;
            var dbComment = query.Single();
            dbComment.Content.ShouldBe(Comment.Content);
            dbComment.ContentId.ShouldBe(Comment.ContentId);
            dbComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
            dbComment.IsDeleted.ShouldBeFalse();

            var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
            cacheComment.ShouldNotBeNull();
            cacheComment.Content.ShouldBe(Comment.Content);
            cacheComment.ContentId.ShouldBe(Comment.ContentId);
            cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
            cacheComment.IsDeleted.ShouldBeFalse();
        }

        [Theory]
        [InlineData(true)]
        public void Update_WhenEntityIsValid_ShouldPersistRecord(bool useTransaction)
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = useTransaction ? new TransactionScope() : null;
            using var scope = factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            _hubContentPostTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);
            _socialProfileTests.Insert_WhenEntityIsValid_ShouldPersistRecord(false);

            superRepository.ContentComments.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, Comment));

            //private scope
            {
                var query = from a in db.ContentComments.AsNoTracking() where a.Id == Comment.Id select a;
                var dbComment = query.Single();
                dbComment.Content.ShouldBe(Comment.Content);
                dbComment.ContentId.ShouldBe(Comment.ContentId);
                dbComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);

                var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
                cacheComment.ShouldNotBeNull();
                cacheComment.Content.ShouldBe(Comment.Content);
                cacheComment.ContentId.ShouldBe(Comment.ContentId);
                cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
            }

            Comment.Content = "<p>Updated Content</p>";

            superRepository.ContentComments.SmartUpdate(new(TestFacebookProfile, DateTime.UtcNow, Comment));

            //private scope
            {
                var query = from a in db.ContentComments.AsNoTracking() where a.Id == Comment.Id select a;
                var dbComment = query.Single();
                dbComment.Content.ShouldBe(Comment.Content);
                dbComment.ContentId.ShouldBe(Comment.ContentId);
                dbComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
                dbComment.IsDeleted.ShouldBeFalse();

                var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
                cacheComment.ShouldNotBeNull();
                cacheComment.Content.ShouldBe(Comment.Content);
                cacheComment.ContentId.ShouldBe(Comment.ContentId);
                cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
                cacheComment.IsDeleted.ShouldBeFalse();
            }
        }
    }
}
