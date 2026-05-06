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
        public HubContentTests() : base()
        {
            
        }

        [Fact]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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
                t2.Tags.Add(new() { ContentId = t2.Id, Tag = "Test tag", Slug = "Test tag".GetSlug() });
            }

            t2.Entity.AuthorId = TestUser.Id;
            t2.Entity.Content = "<p>Test content</p>";
            t2.Entity.Description = "Test description";
            t2.Entity.LanguageWrittenOn = "en-US";
            t2.Entity.Slug = "Test title".GetSlug();
            t2.Entity.Title = "Test title";

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<T2>>();
            var hub = hubRepository.Save(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");

            var deleteHub = hubRepository.Delete(t2, TestUser);

            deleteHub.ShouldNotBeNull();
            deleteHub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeTrue();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeTrue();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

            if (t2 is HubContentPost or HubContentVideo)
            {
                cacheHubContent.Categories.Count.ShouldBe(1);
                cacheHubContent.Categories[0].CategoryId.ShouldBe(TestCategory.Id);
                cacheHubContent.Categories[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Categories[0].Version.ShouldBe(t2.Categories[0].Version);

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags = cacheHubContent.Tags
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking()
                    .Where(x => x.ContentId == t2.Entity.Id)
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).OrderBy(x => x.DateCreated).ToList();
            dbHubHistories.Count.ShouldBe(2);
            dbHubHistories[0].Description.ShouldBe("Content created");
            dbHubHistories[1].Description.ShouldBe("Content deleted");
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");
        }

        [Fact]
        public void Publish_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");

            hub = hubRepository.Publish(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeTrue();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.DatePublished.HasValue.ShouldBeTrue();
            dbEntity.DatePublished.Value.ShouldBeGreaterThanOrEqualTo(dbEntity.DateCreated);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeTrue();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.DatePublished.HasValue.ShouldBeTrue();
            cacheHubContent.Entity.DatePublished.Value.ShouldBeGreaterThanOrEqualTo(cacheHubContent.Entity.DateCreated);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);
        }

        [Fact]
        public void Purge_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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
                t2.Tags.Add(new() { ContentId = t2.Id, Tag = "Test tag", Slug = "Test tag".GetSlug() });
            }

            t2.Entity.AuthorId = TestUser.Id;
            t2.Entity.Content = "<p>Test content</p>";
            t2.Entity.Description = "Test description";
            t2.Entity.LanguageWrittenOn = "en-US";
            t2.Entity.Slug = "Test title".GetSlug();
            t2.Entity.Title = "Test title";

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<T2>>();
            var hub = hubRepository.Save(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");

            var deleteHub = hubRepository.Delete(t2, TestUser);

            deleteHub.ShouldNotBeNull();
            deleteHub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeTrue();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeTrue();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

            if (t2 is HubContentPost or HubContentVideo)
            {
                cacheHubContent.Categories.Count.ShouldBe(1);
                cacheHubContent.Categories[0].CategoryId.ShouldBe(TestCategory.Id);
                cacheHubContent.Categories[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Categories[0].Version.ShouldBe(t2.Categories[0].Version);

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags = cacheHubContent.Tags
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking()
                    .Where(x => x.ContentId == t2.Entity.Id)
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).OrderBy(x => x.DateCreated).ToList();
            dbHubHistories.Count.ShouldBe(2);
            dbHubHistories[0].Description.ShouldBe("Content created");
            dbHubHistories[1].Description.ShouldBe("Content deleted");

            var purgeHub = hubRepository.Purge(t2, TestUser);
            purgeHub.ShouldNotBeNull();
            purgeHub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldBeNull();

            var dbPurgedCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbPurgedCategories.ShouldBeEmpty();

            var dbPurgedTags = db.Set<OrigamiContentTag>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbPurgedTags.ShouldBeEmpty();

            dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).OrderBy(x => x.DateCreated).ToList();
            dbHubHistories.ShouldBeEmpty();

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Id.ShouldBe(Guid.Empty);
            cacheHubContent.Entity.Id.ShouldNotBe(t2.Entity.Id);
            cacheHubContent.Parent.ShouldBeNull();
            cacheHubContent.Children.ShouldBeEmpty();
            cacheHubContent.Categories.ShouldBeEmpty();
            cacheHubContent.Tags.ShouldBeEmpty();
        }

        [Fact]
        public void Restore_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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
                t2.Tags.Add(new() { ContentId = t2.Id, Tag = "Test tag", Slug = "Test tag".GetSlug() });
            }

            t2.Entity.AuthorId = TestUser.Id;
            t2.Entity.Content = "<p>Test content</p>";
            t2.Entity.Description = "Test description";
            t2.Entity.LanguageWrittenOn = "en-US";
            t2.Entity.Slug = "Test title".GetSlug();
            t2.Entity.Title = "Test title";

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<T2>>();
            var hub = hubRepository.Save(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");

            var deleteHub = hubRepository.Delete(t2, TestUser);

            deleteHub.ShouldNotBeNull();
            deleteHub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeTrue();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeTrue();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

            if (t2 is HubContentPost or HubContentVideo)
            {
                cacheHubContent.Categories.Count.ShouldBe(1);
                cacheHubContent.Categories[0].CategoryId.ShouldBe(TestCategory.Id);
                cacheHubContent.Categories[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Categories[0].Version.ShouldBe(t2.Categories[0].Version);

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags = cacheHubContent.Tags
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking()
                    .Where(x => x.ContentId == t2.Entity.Id)
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).OrderBy(x => x.DateCreated).ToList();
            dbHubHistories.Count.ShouldBe(2);
            dbHubHistories[0].Description.ShouldBe("Content created");
            dbHubHistories[1].Description.ShouldBe("Content deleted");

            var restoreHub = hubRepository.Restore(t2, TestUser);

            restoreHub.ShouldNotBeNull();
            restoreHub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

            if (t2 is HubContentPost or HubContentVideo)
            {
                cacheHubContent.Categories.Count.ShouldBe(1);
                cacheHubContent.Categories[0].CategoryId.ShouldBe(TestCategory.Id);
                cacheHubContent.Categories[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Categories[0].Version.ShouldBe(t2.Categories[0].Version);

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags = cacheHubContent.Tags
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking()
                    .Where(x => x.ContentId == t2.Entity.Id)
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).OrderBy(x => x.DateCreated).ToList();
            dbHubHistories.Count.ShouldBe(3);
            dbHubHistories[0].Description.ShouldBe("Content created");
            dbHubHistories[1].Description.ShouldBe("Content deleted");
            dbHubHistories[2].Description.ShouldBe("Content restored");
        }
        [Fact]
        public void Unpublish_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");

            hub = hubRepository.Publish(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeTrue();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.DatePublished.HasValue.ShouldBeTrue();
            dbEntity.DatePublished.Value.ShouldBeGreaterThanOrEqualTo(dbEntity.DateCreated);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeTrue();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.DatePublished.HasValue.ShouldBeTrue();
            cacheHubContent.Entity.DatePublished.Value.ShouldBeGreaterThanOrEqualTo(cacheHubContent.Entity.DateCreated);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

            hub = hubRepository.Unpublish(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.DatePublished.HasValue.ShouldBeFalse();
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.DatePublished.HasValue.ShouldBeFalse();
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
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
                t2.Tags.Add(new() { ContentId = t2.Id, Tag = "Test tag", Slug = "Test tag".GetSlug() });
            }

            t2.Entity.AuthorId = TestUser.Id;
            t2.Entity.Content = "<p>Test content</p>";
            t2.Entity.Description = "Test description";
            t2.Entity.LanguageWrittenOn = "en-US";
            t2.Entity.Slug = "Test title".GetSlug();
            t2.Entity.Title = "Test title";

            var hubRepository = scope.ServiceProvider.GetRequiredService<IHubContentRepository<T2>>();
            var hub = hubRepository.Save(t2, TestUser);

            hub.ShouldNotBeNull();
            hub.Ok.ShouldBeTrue();

            var dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.IsDeleted.ShouldBeFalse();
            dbEntity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            dbEntity.IsPublished.ShouldBeFalse();
            dbEntity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            var cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Entity.IsDeleted.ShouldBeFalse();
            cacheHubContent.Entity.IsDeleted.ShouldBe(t2.Entity.IsDeleted);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.IsPublished.ShouldBeFalse();
            cacheHubContent.Entity.IsPublished.ShouldBe(t2.Entity.IsPublished);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

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

                var dbCategories = db.Set<OrigamiContentCategory>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags.Count.ShouldBe(1);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);

                var dbTags = db.Set<OrigamiContentTag>().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbTags.Count.ShouldBe(1);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
            }

            var dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
            dbHubHistories.Count.ShouldBe(1);
            dbHubHistories[0].Description.ShouldBe("Content created");

            t2.Entity.Content = "<p>Updated test content</p>";
            t2.Entity.Description = "Updated test Description";
            t2.Entity.LanguageWrittenOn = "en-US";
            t2.Entity.Slug = "Updated test title".GetSlug();
            t2.Entity.Title = "Updated test title";

            if (t2 is HubContentPost or HubContentVideo)
            {
                t2.Tags.Add(new() { ContentId = t2.Id, Tag = "Test tag II", Slug = "Test tag II".GetSlug() });
            }

            var updateHub = hubRepository.Save(t2, TestUser);

            updateHub.ShouldNotBeNull();
            updateHub.Ok.ShouldBeTrue();

            dbEntity = db.Set<T1>().AsNoTracking().Id(t2.Entity.Id);
            dbEntity.ShouldNotBeNull();
            dbEntity.Content.ShouldBe(t2.Entity.Content);
            dbEntity.Description.ShouldBe(t2.Entity.Description);
            dbEntity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            dbEntity.Slug.ShouldBe(t2.Entity.Slug);
            dbEntity.Title.ShouldBe(t2.Entity.Title);
            dbEntity.Version.ShouldBe(t2.Entity.Version);

            cacheHubContent = hubRepository.Get(t2);
            cacheHubContent.Entity.Content.ShouldBe(t2.Entity.Content);
            cacheHubContent.Entity.Description.ShouldBe(t2.Entity.Description);
            cacheHubContent.Entity.LanguageWrittenOn.ShouldBe(t2.Entity.LanguageWrittenOn);
            cacheHubContent.Entity.Slug.ShouldBe(t2.Entity.Slug);
            cacheHubContent.Entity.Title.ShouldBe(t2.Entity.Title);
            cacheHubContent.Categories.Count.ShouldBe(t2.Categories.Count);
            cacheHubContent.Tags.Count.ShouldBe(t2.Tags.Count);
            cacheHubContent.Entity.Version.ShouldBe(t2.Entity.Version);

            if (t2 is HubContentPost or HubContentVideo)
            {
                cacheHubContent.Categories.Count.ShouldBe(1);
                cacheHubContent.Categories[0].CategoryId.ShouldBe(TestCategory.Id);
                cacheHubContent.Categories[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Categories[0].Version.ShouldBe(t2.Categories[0].Version);

                var dbCategories = db.Set<OrigamiContentCategory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).ToList();
                dbCategories.Count.ShouldBe(1);
                dbCategories[0].CategoryId.ShouldBe(TestCategory.Id);
                dbCategories[0].ContentId.ShouldBe(t2.Entity.Id);
                dbCategories[0].Version.ShouldBe(t2.Categories[0].Version);

                cacheHubContent.Tags = cacheHubContent.Tags
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                cacheHubContent.Tags.Count.ShouldBe(2);
                cacheHubContent.Tags[0].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                cacheHubContent.Tags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                cacheHubContent.Tags[0].Version.ShouldBe(t2.Tags[0].Version);
                cacheHubContent.Tags[1].ContentId.ShouldBe(t2.Entity.Id);
                cacheHubContent.Tags[1].Tag.ShouldBe(t2.Tags[1].Tag);
                cacheHubContent.Tags[1].Slug.ShouldBe(t2.Tags[1].Slug);
                cacheHubContent.Tags[1].Version.ShouldBe(t2.Tags[1].Version);

                var dbTags = db.Set<OrigamiContentTag>().AsNoTracking()
                    .Where(x => x.ContentId == t2.Entity.Id)
                    .OrderBy(x => x.ContentId)
                    .ThenBy(x => x.Tag)
                    .ToList();

                dbTags.Count.ShouldBe(2);
                dbTags[0].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[0].Tag.ShouldBe(t2.Tags[0].Tag);
                dbTags[0].Slug.ShouldBe(t2.Tags[0].Slug);
                dbTags[0].Version.ShouldBe(t2.Tags[0].Version);
                dbTags[1].ContentId.ShouldBe(t2.Entity.Id);
                dbTags[1].Tag.ShouldBe(t2.Tags[1].Tag);
                dbTags[1].Slug.ShouldBe(t2.Tags[1].Slug);
                dbTags[1].Version.ShouldBe(t2.Tags[1].Version);
            }

            dbHubHistories = db.Set<OrigamiContentHistory>().AsNoTracking().Where(x => x.ContentId == t2.Entity.Id).OrderBy(x => x.DateCreated).ToList();
            dbHubHistories.Count.ShouldBe(2);
            dbHubHistories[0].Description.ShouldBe("Content created");
            dbHubHistories[1].Description.ShouldBe("Content saved");
        }
    }
}
