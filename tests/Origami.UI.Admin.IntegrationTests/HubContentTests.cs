using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public abstract class HubContentTests<T1, T2> :
        CustomClassFixture
        where T1 : OrigamiContent  
        where T2 : HubContent<T1>
    {
        public HubContentTests(CustomWebApplicationFactory factory) : base(factory)
        {
            
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<IDbContextFactory<OrigamiDbContext>>().CreateDbContext();
            var superRepository = scope.ServiceProvider.GetRequiredService<ISuperRepository>();

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            var t2 = Activator.CreateInstance<T2>()!;
            if (t2 is HubContentPage or HubContentPost or HubContentVideo)
            {
                t2.Entity.BlogId = TestBlog.Id;
            }
            if (t2 is HubContentPost or HubContentVideo)
            {
                t2.Categories.Add(new() { CategoryId = TestCategory.Id, ContentId = t2.Id });
                t2.Tags.Add(new() { ContentId = t2.Id, Tag = "Test Tag", Slug = "Test Tag".GetSlug() });
            }

            t2.Entity.AuthorId = TestUser.Id;
            t2.Entity.Content = "<p>Test content</p>";
            t2.Entity.Description = "Test Description";
            t2.Entity.LanguageWrittenOn = "en-US";
            t2.Entity.Slug = "Test Title".GetSlug();
            t2.Entity.Title = "Test Title";

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<T2>>();
            var hub = hubRepository.Save(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);

            if (t2 is HubContentPage)
            {
                cacheHubContent.Categories.Count.ShouldBe(0);
                cacheHubContent.Tags.Count.ShouldBe(0);
            }

            if (t2 is HubContentPost or HubContentVideo)
            {
                cacheHubContent.Categories.Count.ShouldBe(1);
                cacheHubContent.Categories[0].CategoryId.ShouldBe(TestCategory.Id);
                cacheHubContent.Categories[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Categories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);
            }
        }
    }
}
