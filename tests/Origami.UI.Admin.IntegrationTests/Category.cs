using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class Category : CustomClassFixture
    {
        public Category(CustomWebApplicationFactory factory) : base(factory)
        {
            
        }

        [Fact]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);
            this.CreateTestCategory(TestCategory);

            using var db = _superRepository.DbContextFactory.CreateDbContext();
            var category = db.Categories.AsNoTracking().Id(TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);
            category.IsDeleted.ShouldBe(TestCategory.IsDeleted);

            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var result = categoryRepository.SmartDelete(category.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeTrue();

            var categoryAfterDelete = categoryRepository.ReadFromDatabase(TestCategory);
            categoryAfterDelete.ShouldNotBeNull();
            categoryAfterDelete.Name.ShouldBe(TestCategory.Name);
            categoryAfterDelete.DateCreated.ShouldBe(TestCategory.DateCreated);
            categoryAfterDelete.NanoId.ShouldBe(TestCategory.NanoId);
            categoryAfterDelete.IsDeleted.ShouldBe(true);

            var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id)!;
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe(TestCategory.Name);
            cacheCategory.DateCreated.ShouldBe(TestCategory.DateCreated);
            cacheCategory.NanoId.ShouldBe(TestCategory.NanoId);
            cacheCategory.IsDeleted.ShouldBe(true);

            categoryRepository.PurgeCache(TestCategory);
        }

        [Fact]
        public void Insert_When3CategoriesAreLinkedToEachOther_ShouldPersistRecords()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            using var db = _superRepository.DbContextFactory.CreateDbContext();

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

                var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(memCategory.Id)!;
                cacheCategory.ShouldNotBeNull();
                cacheCategory.Name.ShouldBe(memCategory.Name);
                cacheCategory.DateCreated.ShouldBe(memCategory.DateCreated);
                cacheCategory.NanoId.ShouldBe(memCategory.NanoId);
                cacheCategory.IsDeleted.ShouldBe(memCategory.IsDeleted);
            }

            categoryRepository.PurgeCache(TestCategoryA);
            categoryRepository.PurgeCache(TestCategoryB);
            categoryRepository.PurgeCache(TestCategoryC);
        }

        [Fact]
        public void Insert_When3CategoriesAreLoopedToEachOther_ShouldFail()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            using var db = _superRepository.DbContextFactory.CreateDbContext();

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

                var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(memCategory.Id)!;
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

            var cacheAfterLoop = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategoryA.Id);
            cacheAfterLoop.ShouldNotBeNull();
            cacheAfterLoop.ParentId.ShouldBeNull();

            categoryRepository.PurgeCache(TestCategoryA);
            categoryRepository.PurgeCache(TestCategoryB);
            categoryRepository.PurgeCache(TestCategoryC);
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);
            this.CreateTestCategory(TestCategory);

            using var db = _superRepository.DbContextFactory.CreateDbContext();

            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id);
            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);

            var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe(TestCategory.Name);
            cacheCategory.DateCreated.ShouldBe(TestCategory.DateCreated);
            cacheCategory.NanoId.ShouldBe(TestCategory.NanoId);
            cacheCategory.IsDeleted.ShouldBe(false);

            _scope.ServiceProvider.GetRequiredService<ICategoryRepository>().PurgeCache(TestCategory);
        }

        [Fact]
        public void Insert_WhenNameExceeds50Characters_ShouldFail()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            using var db = _superRepository.DbContextFactory.CreateDbContext();
            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
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

            var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldBeNull();
        }

        [Fact]
        public void Purge_WhenEntityIsDeleted_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);
            this.CreateTestCategory(TestCategory);

            using var db = _scope.ServiceProvider.GetService<IDbContextFactory<OrigamiDbContext>>()!.CreateDbContext();
            var category = db.Categories.AsNoTracking().Id(TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);
            category.IsDeleted.ShouldBe(TestCategory.IsDeleted);

            var categoryRepository = _scope.ServiceProvider.GetService<ICategoryRepository>()!;
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

            var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldBeNull();
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);
            this.CreateTestCategory(TestCategory);

            using var db = _superRepository.DbContextFactory.CreateDbContext();
            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
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

            var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe("Updated category name");

            categoryRepository.PurgeCache(TestCategory);
        }

        [Fact]
        public void Update_WhenNameExceeds50Characters_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);
            this.CreateTestCategory(TestCategory);

            using var db = _superRepository.DbContextFactory.CreateDbContext();
            var categoryRepository = _scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
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

            var cacheCategory = _superRepository.MyMemoryCache.Read<OrigamiCategory>().Id(TestCategory.Id);
            cacheCategory.ShouldNotBeNull();
            cacheCategory.Name.ShouldBe("Test category");

            categoryRepository.PurgeCache(TestCategory);
        }
    }
}
