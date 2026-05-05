using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class CategoryTests : CustomClassFixture
    {
        public CategoryTests() : base()
        {
            
        }

        [Fact]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var category = db.Categories.AsNoTracking().Id(TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);
            category.IsDeleted.ShouldBe(TestCategory.IsDeleted);

            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var result = categoryRepository.SmartDelete(category.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var categoryAfterDelete = categoryRepository.ReadFromDatabase(TestCategory);
            categoryAfterDelete.ShouldNotBeNull();
            categoryAfterDelete.Name.ShouldBe(TestCategory.Name);
            categoryAfterDelete.DateCreated.ShouldBe(TestCategory.DateCreated);
            categoryAfterDelete.NanoId.ShouldBe(TestCategory.NanoId);
            categoryAfterDelete.IsDeleted.ShouldBe(true);

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id)!;
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe(TestCategory.Name);
            cacheCategory.DateCreated.ShouldBe(TestCategory.DateCreated);
            cacheCategory.NanoId.ShouldBe(TestCategory.NanoId);
            cacheCategory.IsDeleted.ShouldBe(true);

            cacheCategory.Version.ShouldBe(categoryAfterDelete.Version);
            cacheCategory.Version.ShouldBe(category.Version);
        }

        [Fact]
        public void Insert_When3CategoriesAreLinkedToEachOther_ShouldPersistRecords()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            using var db = superRepository.DbContextFactory.CreateDbContext();

            var resultA = categoryRepository.SmartSave(TestCategoryA.GetContext(TestUser), true);
            var resultB = categoryRepository.SmartSave(TestCategoryB.GetContext(TestUser), true);
            var resultC = categoryRepository.SmartSave(TestCategoryC.GetContext(TestUser), true);

            IList<Result<OrigamiCategory>> results = [resultA, resultB, resultC];

            foreach (var result in results)
            {
                result.ShouldNotBeNull();
                result.Ok.ShouldBeTrue();
                result.Entity.ShouldNotBeNull();
                result.Messages.ShouldNotBeNull();
                result.Messages.Count.ShouldBe(0);
                var category = db.Categories.AsNoTracking().Id(result.Entity.Id)!;
                var memCategory = results.IndexOf(result) switch
                {
                    0 => TestCategoryA,
                    1 => TestCategoryB,
                    2 => TestCategoryC,
                    _ => throw new Exception("Invalid category index")
                };
                category.BlogId.ShouldBe(memCategory.BlogId);
                category.ParentId.ShouldBe(memCategory.ParentId);
                category.Id.ShouldBe(memCategory.Id);
                category.Name.ShouldBe(memCategory.Name);
                category.DateCreated.ShouldBe(memCategory.DateCreated);
                category.NanoId.ShouldBe(memCategory.NanoId);
                category.IsDeleted.ShouldBe(memCategory.IsDeleted);

                var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(memCategory.Id)!;
                cacheCategory.ShouldNotBeNull();
                cacheCategory.Name.ShouldBe(memCategory.Name);
                cacheCategory.DateCreated.ShouldBe(memCategory.DateCreated);
                cacheCategory.NanoId.ShouldBe(memCategory.NanoId);
                cacheCategory.IsDeleted.ShouldBe(memCategory.IsDeleted);

                cacheCategory.Version.ShouldBe(memCategory.Version);
                cacheCategory.Version.ShouldBe(category.Version);
            }
        }

        [Fact]
        public void Insert_When3CategoriesAreLoopedToEachOther_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            using var db = superRepository.DbContextFactory.CreateDbContext();

            var resultA = categoryRepository.SmartSave(TestCategoryA.GetContext(TestUser), true);
            var resultB = categoryRepository.SmartSave(TestCategoryB.GetContext(TestUser), true);
            var resultC = categoryRepository.SmartSave(TestCategoryC.GetContext(TestUser), true);

            IList<Result<OrigamiCategory>> results = [resultA, resultB, resultC];

            foreach (var result in results)
            {
                result.ShouldNotBeNull();
                result.Ok.ShouldBeTrue();
                result.Entity.ShouldNotBeNull();
                result.Messages.ShouldNotBeNull();
                result.Messages.Count.ShouldBe(0);
                var category = db.Categories.Id(result.Entity.Id)!;
                var memCategory = results.IndexOf(result) switch
                {
                    0 => TestCategoryA,
                    1 => TestCategoryB,
                    2 => TestCategoryC,
                    _ => throw new Exception("Invalid category index")
                };
                category.BlogId.ShouldBe(memCategory.BlogId);
                category.ParentId.ShouldBe(memCategory.ParentId);
                category.Id.ShouldBe(memCategory.Id);
                category.Name.ShouldBe(memCategory.Name);
                category.DateCreated.ShouldBe(memCategory.DateCreated);
                category.NanoId.ShouldBe(memCategory.NanoId);
                category.IsDeleted.ShouldBe(memCategory.IsDeleted);

                var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(memCategory.Id)!;
                cacheCategory.ShouldNotBeNull();
                cacheCategory.Name.ShouldBe(memCategory.Name);
                cacheCategory.DateCreated.ShouldBe(memCategory.DateCreated);
                cacheCategory.NanoId.ShouldBe(memCategory.NanoId);
                cacheCategory.IsDeleted.ShouldBe(memCategory.IsDeleted);
            }

            TestCategoryA.ParentId = TestCategoryC.Id;
            var resultLoop = categoryRepository.SmartSave(TestCategoryA.GetContext(TestUser), true);

            resultLoop.ShouldNotBeNull();
            resultLoop.Ok.ShouldBeFalse();
            resultLoop.Messages.ShouldNotBeNull();
            resultLoop.Messages.Count.ShouldBe(1);
            resultLoop.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            resultLoop.Messages[0].Message.ShouldBe("Loop in relationships are not allowed");

            var categoryAfterLoop = db.Categories.AsNoTracking().Id(TestCategoryA.Id);
            categoryAfterLoop.ShouldNotBeNull();
            categoryAfterLoop.ParentId.ShouldBeNull();

            var cacheAfterLoop = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategoryA.Id);
            cacheAfterLoop.ShouldNotBeNull();
            cacheAfterLoop.ParentId.ShouldBeNull();
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();

            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id);
            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe(TestCategory.Name);
            cacheCategory.DateCreated.ShouldBe(TestCategory.DateCreated);
            cacheCategory.NanoId.ShouldBe(TestCategory.NanoId);
            cacheCategory.IsDeleted.ShouldBe(false);

            cacheCategory.Version.ShouldBe(TestCategory.Version);
            cacheCategory.Version.ShouldBe(category.Version);
        }

        [Fact]
        public void Insert_WhenNameIsTooLarge_ShouldFail()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var result = categoryRepository.SmartSave(TestCategoryWithBigName.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(2);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Name cannot exceed 50 characters");
            result.Messages[1].Message.ShouldBe("Slug cannot exceed 50 characters");

            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategoryWithBigName.Id);
            category.ShouldBeNull();

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldBeNull();
        }

        [Fact]
        public void Purge_WhenEntityIsDeleted_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var category = db.Categories.AsNoTracking().Id(TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);
            category.IsDeleted.ShouldBe(TestCategory.IsDeleted);

            var categoryRepository = scope.ServiceProvider.GetService<ICategoryRepository>()!;
            var delete = categoryRepository.SmartDelete(category.GetContext(TestUser), true);
            delete.ShouldNotBeNull();
            delete.Ok.ShouldBeTrue();

            var categoryAfterDelete = categoryRepository.ReadFromDatabase(TestCategory);
            categoryAfterDelete.ShouldNotBeNull();
            categoryAfterDelete.Name.ShouldBe(TestCategory.Name);
            categoryAfterDelete.DateCreated.ShouldBe(TestCategory.DateCreated);
            categoryAfterDelete.NanoId.ShouldBe(TestCategory.NanoId);
            categoryAfterDelete.IsDeleted.ShouldBe(true);

            var purge = categoryRepository.SmartPurge(categoryAfterDelete.GetContext(TestUser), true);
            purge.ShouldNotBeNull();
            purge.Ok.ShouldBeTrue();

            var categoryAfterPurge = categoryRepository.ReadFromDatabase(TestCategory);
            categoryAfterPurge.ShouldBeNull();

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldBeNull();
        }

        [Fact]
        public void Restore_WhenEntityIsDeleted_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var category = db.Categories.AsNoTracking().Id(TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);
            category.IsDeleted.ShouldBe(TestCategory.IsDeleted);

            var categoryRepository = scope.ServiceProvider.GetService<ICategoryRepository>()!;
            var delete = categoryRepository.SmartDelete(category.GetContext(TestUser), true);
            delete.ShouldNotBeNull();
            delete.Ok.ShouldBeTrue();

            var categoryAfterDelete = categoryRepository.ReadFromDatabase(TestCategory);
            categoryAfterDelete.ShouldNotBeNull();
            categoryAfterDelete.Name.ShouldBe(TestCategory.Name);
            categoryAfterDelete.DateCreated.ShouldBe(TestCategory.DateCreated);
            categoryAfterDelete.NanoId.ShouldBe(TestCategory.NanoId);
            categoryAfterDelete.IsDeleted.ShouldBe(true);

            var restore = categoryRepository.SmartRestore(categoryAfterDelete.GetContext(TestUser), true);
            restore.ShouldNotBeNull();
            restore.Ok.ShouldBeTrue();

            var categoryAfterRestore = categoryRepository.ReadFromDatabase(TestCategory);
            categoryAfterRestore.ShouldNotBeNull();
            categoryAfterRestore.Name.ShouldBe(TestCategory.Name);
            categoryAfterRestore.DateCreated.ShouldBe(TestCategory.DateCreated);
            categoryAfterRestore.NanoId.ShouldBe(TestCategory.NanoId);
            categoryAfterRestore.IsDeleted.ShouldBe(false);

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe(TestCategory.Name);
            cacheCategory.DateCreated.ShouldBe(TestCategory.DateCreated);
            cacheCategory.NanoId.ShouldBe(TestCategory.NanoId);
            cacheCategory.IsDeleted.ShouldBeFalse();

            cacheCategory.Version.ShouldBe(categoryAfterDelete.Version);
            cacheCategory.Version.ShouldBe(categoryAfterRestore.Version);
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);

            category.Name = "Updated category name";

            categoryRepository
                .SmartSave(category.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test category: {r.GetMessages()}"));

            var categoryAfterUpdate = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id); 
            categoryAfterUpdate.ShouldNotBeNull();
            categoryAfterUpdate.Name.ShouldBe("Updated category name");

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe("Updated category name");

            cacheCategory.Version.ShouldBe(categoryAfterUpdate.Version);
            cacheCategory.Version.ShouldBe(category.Version);
        }

        [Fact]
        public void Update_WhenNameIsTooLarge_ShouldPersistRecord()
        {
            using var factory = new CustomWebApplicationFactory();
            using var transaction = new TransactionScope();
            using var scope = factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);
            scope.CreateTestCategory(TestCategory, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);

            category.Name = "Updated category name " + new string('a', 100);

            var result = categoryRepository.SmartSave(category.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(2);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Name cannot exceed 50 characters");
            result.Messages[1].Message.ShouldBe("Slug cannot exceed 50 characters");

            var categoryAfterUpdate = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id);
            categoryAfterUpdate.ShouldNotBeNull();
            categoryAfterUpdate.Name.ShouldBe("Test category");

            var cacheCategory = superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe("Test category");

            cacheCategory.Version.ShouldBe(categoryAfterUpdate.Version);
            cacheCategory.Version.ShouldBe(category.Version);
        }
    }
}
