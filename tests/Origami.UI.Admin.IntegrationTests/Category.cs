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
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = _scope.ServiceProvider.GetService<ICategoryRepository>()!;
            using var db = categoryRepository.DbContextFactory.CreateDbContext();

            categoryRepository
                .SmartSave(TestCategory.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test category: {r.GetMessages()}"));

            var category = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == TestCategory.Id);

            category.ShouldNotBeNull();
            category.Name.ShouldBe(TestCategory.Name);
            category.DateCreated.ShouldBe(TestCategory.DateCreated);
            category.NanoId.ShouldBe(TestCategory.NanoId);
        }

        [Fact]
        public void Insert_WhenNameExceeds50Characters_ShouldFail()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = _scope.ServiceProvider.GetService<ICategoryRepository>()!;
            using var db = categoryRepository.DbContextFactory.CreateDbContext();
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
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = _scope.ServiceProvider.GetService<ICategoryRepository>()!;
            using var db = categoryRepository.DbContextFactory.CreateDbContext();

            categoryRepository
                .SmartSave(TestCategory.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test category: {r.GetMessages()}"));

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
        }

        [Fact]
        public void Update_WhenNameExceeds50Characters_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestBlog(TestBlog, TestRole, TestUser);

            var categoryRepository = _scope.ServiceProvider.GetService<ICategoryRepository>()!;
            using var db = categoryRepository.DbContextFactory.CreateDbContext();

            categoryRepository
                .SmartSave(TestCategory.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test category: {r.GetMessages()}"));

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
        }
    }
}
