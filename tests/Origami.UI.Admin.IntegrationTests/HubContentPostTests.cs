using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class HubContentPostTests : HubContentTests<OrigamiPost, HubContentPost>
    {
        public HubContentPostTests() : base()
        {

        }

        [Fact]
        public void Insert_When3PostsAreLoopedToEachOther_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategoryA, TestUser);
            scope.CreateTestCategory(TestCategoryB, TestUser);
            scope.CreateTestCategory(TestCategoryC, TestUser);

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<HubContentPost>>();
            using var db = superRepository.DbContextFactory.CreateDbContext();

            var hubA = new HubContentPost { Entity = TestPostA, };
            var hubB = new HubContentPost { Entity = TestPostB, };
            var hubC = new HubContentPost { Entity = TestPostC, };

            var resultA = hubRepository.Save(hubA, TestUser);
            var resultB = hubRepository.Save(hubB, TestUser);
            var resultC = hubRepository.Save(hubC, TestUser);

            IList<Result<HubContentPost>> results = [resultA, resultB, resultC];

            foreach (var result in results)
            {
                result.ShouldNotBeNull();
                result.Ok.ShouldBeTrue();
                result.Entity.ShouldNotBeNull();
                result.Messages.ShouldNotBeNull();
                result.Messages.Count.ShouldBe(1);
                result.Messages[0].Message.ShouldBe("Yay! Everything went smoothly");
                result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Success);
                var post = db.Posts.Id(result.Entity.Id)!;
                var memPost = results.IndexOf(result) switch
                {
                    0 => TestPostA,
                    1 => TestPostB,
                    2 => TestPostC,
                    _ => throw new Exception("Invalid post index")
                };
                post.BlogId.ShouldBe(memPost.BlogId);
                post.ParentId.ShouldBe(memPost.ParentId);
                post.Id.ShouldBe(memPost.Id);
                post.Title.ShouldBe(memPost.Title);
                post.Content.ShouldBe(memPost.Content);
                post.DateCreated.ShouldBe(memPost.DateCreated);
                post.NanoId.ShouldBe(memPost.NanoId);
                post.IsDeleted.ShouldBe(memPost.IsDeleted);

                var cachePost = superRepository.MyMemoryCache.Read<OrigamiPost>().Id(memPost.Id)!;
                cachePost.ShouldNotBeNull();
                cachePost.Title.ShouldBe(memPost.Title);
                cachePost.Content.ShouldBe(memPost.Content);
                cachePost.DateCreated.ShouldBe(memPost.DateCreated);
                cachePost.NanoId.ShouldBe(memPost.NanoId);
                cachePost.IsDeleted.ShouldBe(memPost.IsDeleted);
            }

            hubA.Entity.ParentId = TestPostC.Id;
            hubA.Categories.Add(new() { CategoryId = TestCategoryA.Id, ContentId = hubA.Id });
            hubA.Categories.Add(new() { CategoryId = TestCategoryB.Id, ContentId = hubA.Id });
            hubA.Categories.Add(new() { CategoryId = TestCategoryC.Id, ContentId = hubA.Id });

            var resultLoop = hubRepository.Save(hubA, TestUser);

            resultLoop.ShouldNotBeNull();
            resultLoop.Ok.ShouldBeFalse();
            resultLoop.Messages.ShouldNotBeNull();
            resultLoop.Messages.Count.ShouldBe(1);
            resultLoop.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultLoop.Messages[0].Message.ShouldBe("Loop in relationships are not allowed");

            var postAfterLoop = db.Posts.AsNoTracking().Id(TestPostA.Id);
            postAfterLoop.ShouldNotBeNull();
            postAfterLoop.ParentId.ShouldBeNull();

            var cacheAfterLoop = superRepository.MyMemoryCache.Read<OrigamiPost>().Id(TestPostA.Id);
            cacheAfterLoop.ShouldNotBeNull();
            cacheAfterLoop.ParentId.ShouldBeNull();

            var dbCategories = from a in db.ContentCategories.AsNoTracking() where a.ContentId == hubA.Id select a;
            dbCategories.ShouldNotBeNull();
            dbCategories.Count().ShouldBe(0);

            var cacheCategories = superRepository.MyMemoryCache.Read<OrigamiContentCategory>().Where(a => a.ContentId == hubA.Id);
            cacheCategories.ShouldNotBeNull();
            cacheCategories.Count().ShouldBe(0);
        }
    }
}
