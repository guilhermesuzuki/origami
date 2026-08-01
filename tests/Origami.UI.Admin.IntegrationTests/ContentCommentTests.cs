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

            //private scope
            {
                var query = from a in db.SocialProfileDeletesCommentEvents.AsNoTracking()
                            where a.CommentId == Comment.Id
                            where a.SocialProfileId == TestFacebookProfile.Id
                            select a;

                var dbComment = query.Single();
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

            var hub = superRepository.ContentComments.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, Comment));
            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            //private scope
            {
                var query = from a in db.ContentComments.AsNoTracking() where a.Id == Comment.Id select a;
                var dbComment = query.Single();
                dbComment.Content.ShouldBe(Comment.Content);
                dbComment.ContentId.ShouldBe(Comment.ContentId);
                dbComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
                dbComment.IsDeleted.ShouldBeFalse();
            }

            var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
            cacheComment.ShouldNotBeNull();
            cacheComment.Content.ShouldBe(Comment.Content);
            cacheComment.ContentId.ShouldBe(Comment.ContentId);
            cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
            cacheComment.IsDeleted.ShouldBeFalse();

            //private scope
            {
                var query = from a in db.SocialProfileRepliesToContentEvents.AsNoTracking() 
                            where a.ContentId == Comment.ContentId
                            where a.SocialProfileId == TestFacebookProfile.Id
                            select a;

                var dbComment = query.Single();
            }
        }

        [Theory]
        [InlineData(true)]
        public void Reply_WhenEntityIsValid_ShouldPersistRecord(bool useTransaction)
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
                dbComment.IsDeleted.ShouldBeFalse();

                var cacheComment = superRepository.ContentComments.ReadFromCache().Id(Comment.Id);
                cacheComment.ShouldNotBeNull();
                cacheComment.Content.ShouldBe(Comment.Content);
                cacheComment.ContentId.ShouldBe(Comment.ContentId);
                cacheComment.SocialProfileId.ShouldBe(Comment.SocialProfileId);
                cacheComment.IsDeleted.ShouldBeFalse();
            }

            //private scope
            {
                var query = from a in db.SocialProfileRepliesToContentEvents.AsNoTracking()
                            where a.ContentId == Comment.ContentId
                            where a.SocialProfileId == TestFacebookProfile.Id
                            select a;

                var dbComment = query.Single();
            }

            var reply = new OrigamiContentComment
            {
                Content = "<p>This is a reply</p>",
                ContentId = ContentId,
                DateCreated = DateTime.UtcNow,
                Ip = "192.168.0.1",
                IsApproved = true,
                IsBot = true,
                IsDeleted = false,
                IsMobileDevice = false,
                IsSpam = false,
                ParentId = Comment.Id,
                SocialProfileId = TestFacebookId,
            };

            superRepository.ContentComments.SmartCreate(new(TestFacebookProfile, DateTime.UtcNow, reply));

            //private scope
            {
                var query = from a in db.ContentComments.AsNoTracking() where a.Id == reply.Id select a;
                var dbComment = query.Single();
                dbComment.Content.ShouldBe(reply.Content);
                dbComment.ContentId.ShouldBe(reply.ContentId);
                dbComment.SocialProfileId.ShouldBe(reply.SocialProfileId);
                dbComment.IsDeleted.ShouldBeFalse();

                var cacheComment = superRepository.ContentComments.ReadFromCache().Id(reply.Id);
                cacheComment.ShouldNotBeNull();
                cacheComment.Content.ShouldBe(reply.Content);
                cacheComment.ContentId.ShouldBe(reply.ContentId);
                cacheComment.SocialProfileId.ShouldBe(reply.SocialProfileId);
                cacheComment.IsDeleted.ShouldBeFalse();
            }

            //private scope
            {
                var query = from a in db.SocialProfileRepliesToCommentEvents.AsNoTracking()
                            where a.CommentId == reply.Id
                            where a.SocialProfileId == TestFacebookProfile.Id
                            select a;

                var dbComment = query.Single();
            }
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

            //private scope
            {
                var query = from a in db.SocialProfileRepliesToContentEvents.AsNoTracking()
                            where a.ContentId == Comment.ContentId
                            where a.SocialProfileId == TestFacebookProfile.Id
                            select a;

                var dbComment = query.Single();
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

            //private scope
            {
                var query = from a in db.SocialProfileEditsCommentEvents.AsNoTracking()
                            where a.CommentId == Comment.Id
                            where a.SocialProfileId == TestFacebookProfile.Id
                            select a;

                var dbComment = query.Single();
            }
        }
    }
}
