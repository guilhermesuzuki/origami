
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class HubContentPageTests : HubContentTests<OrigamiPage, HubContentPage>
    {
        public HubContentPageTests() : base()
        {

        }

        [Fact]
        public void Insert_When3PagesAreLoopedToEachOther_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<HubContentPage>>();
            using var db = superRepository.DbContextFactory.CreateDbContext();

            var hubA = new HubContentPage { Entity = TestPageA, };
            var hubB = new HubContentPage { Entity = TestPageB, };
            var hubC = new HubContentPage { Entity = TestPageC, };

            var resultA = hubRepository.Save(hubA, TestUser);
            var resultB = hubRepository.Save(hubB, TestUser);
            var resultC = hubRepository.Save(hubC, TestUser);

            IList<Result<HubContentPage>> results = [ resultA, resultB, resultC ];

            foreach (var result in results)
            {
                result.ShouldNotBeNull();
                result.Ok.ShouldBeTrue();
                result.Entity.ShouldNotBeNull();
                result.Messages.ShouldNotBeNull();
                result.Messages.Count.ShouldBe(1);
                result.Messages[0].Message.ShouldBe("Yay! Everything went smoothly");
                result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Success);
                var page = db.Pages.Id(result.Entity.Id)!;
                var memPage = results.IndexOf(result) switch
                {
                    0 => TestPageA,
                    1 => TestPageB,
                    2 => TestPageC,
                    _ => throw new Exception("Invalid page index")
                };
                page.BlogId.ShouldBe(memPage.BlogId);
                page.ParentId.ShouldBe(memPage.ParentId);
                page.Id.ShouldBe(memPage.Id);
                page.Title.ShouldBe(memPage.Title);
                page.Content.ShouldBe(memPage.Content);
                page.DateCreated.ShouldBe(memPage.DateCreated);
                page.NanoId.ShouldBe(memPage.NanoId);
                page.IsDeleted.ShouldBe(memPage.IsDeleted);

                var cachePage = superRepository.MyMemoryCache.Read<OrigamiPage>().Id(memPage.Id)!;
                cachePage.ShouldNotBeNull();
                cachePage.Title.ShouldBe(memPage.Title);
                cachePage.Content.ShouldBe(memPage.Content);
                cachePage.DateCreated.ShouldBe(memPage.DateCreated);
                cachePage.NanoId.ShouldBe(memPage.NanoId);
                cachePage.IsDeleted.ShouldBe(memPage.IsDeleted);
            }

            TestPageA.ParentId = TestPageC.Id;
            var resultLoop = hubRepository.Save(hubA, TestUser);

            resultLoop.ShouldNotBeNull();
            resultLoop.Ok.ShouldBeFalse();
            resultLoop.Messages.ShouldNotBeNull();
            resultLoop.Messages.Count.ShouldBe(1);
            resultLoop.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultLoop.Messages[0].Message.ShouldBe("Loop in relationships are not allowed");

            var pageAfterLoop = db.Pages.AsNoTracking().Id(TestPageA.Id);
            pageAfterLoop.ShouldNotBeNull();
            pageAfterLoop.ParentId.ShouldBeNull();

            var cacheAfterLoop = superRepository.MyMemoryCache.Read<OrigamiPage>().Id(TestPageA.Id);
            cacheAfterLoop.ShouldNotBeNull();
            cacheAfterLoop.ParentId.ShouldBeNull();
        }
    }
}
