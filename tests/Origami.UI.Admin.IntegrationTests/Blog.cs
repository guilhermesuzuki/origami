using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class Blog : CustomClassFixture
    {
        public Blog(CustomWebApplicationFactory factory) : base(factory)
        {

        }

        [Fact]
        public void Activate_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blog.ShouldNotBeNull();
            blog.Name.ShouldBe(TestBlog.Name);
            blog.DateCreated.ShouldBe(TestBlog.DateCreated);
            blog.NanoId.ShouldBe(TestBlog.NanoId);
            blog.IsActive.ShouldBe(TestBlog.IsActive);

            blogRepository
                .Activate(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to activate test blog: {r.GetMessages()}"));

            var activatedBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            activatedBlog.ShouldNotBeNull();
            activatedBlog.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Deactivate_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blog.ShouldNotBeNull();
            blog.Name.ShouldBe(TestBlog.Name);
            blog.DateCreated.ShouldBe(TestBlog.DateCreated);
            blog.NanoId.ShouldBe(TestBlog.NanoId);
            blog.IsActive.ShouldBe(TestBlog.IsActive);

            blogRepository
                .Activate(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to activate test blog: {r.GetMessages()}"));

            var activatedBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            activatedBlog.ShouldNotBeNull();
            activatedBlog.IsActive.ShouldBeTrue();

            blogRepository
                .Deactivate(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to activate test blog: {r.GetMessages()}"));

            var deactivatedBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            deactivatedBlog.ShouldNotBeNull();
            deactivatedBlog.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);
            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;
            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));
            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            dbBlog.ShouldNotBeNull();
            blogRepository
                .SmartDelete(dbBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to delete test blog: {r.GetMessages()}"));
            var dbBlogAfterDelete = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            dbBlogAfterDelete.ShouldNotBeNull();
            dbBlogAfterDelete.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void GetPrimary_WhenEntityIsValid_ShouldNotThrowException()
        {
            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            OrigamiBlog primary = null!;

            var exception = Record.Exception(() => primary = blogRepository.GetPrimary());
            exception.ShouldBeNull();

            primary.ShouldNotBeNull();
            primary.IsPrimary.ShouldBeTrue();
        }

        [Fact]
        public void Insert_WhenNameExceeds255Characters_ShouldFail()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            var result = blogRepository.SmartSave(TestBlogWithBigName.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(2);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Name cannot exceed 255 characters");
            result.Messages[1].Message.ShouldBe("Slug cannot exceed 255 characters");

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlogWithBigName.Id);
            dbBlog.ShouldBeNull();
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(TestBlog.Name);
            dbBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            dbBlog.NanoId.ShouldBe(TestBlog.NanoId);
        }

        [Fact]
        public void Insert_WhenNoPermissions_ShouldFail()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRoleNoPermissions);
            this.CreateTestUser(TestUser, TestRoleNoPermissions);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            var exception = Record.Exception(() => 
                blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}")));

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Failed to create test blog: CreateNewBlogs\r\nYou don't have permission for this feature");

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blog.ShouldBeNull();
        }

        [Fact]
        public void SetPrimary_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(TestBlog.Name);
            dbBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            dbBlog.NanoId.ShouldBe(TestBlog.NanoId);

            var primary = db.Blogs.AsNoTracking().Single(x => x.IsPrimary);

            blogRepository
                .SetPrimary(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to activate test blog: {r.GetMessages()}"));

            var oldPrimary = db.Blogs.AsNoTracking().Id(primary.Id)!;
            var newPrimary = db.Blogs.AsNoTracking().Single(x => x.IsPrimary);

            oldPrimary.Id.ShouldBe(primary.Id);
            newPrimary.Id.ShouldBe(dbBlog.Id);
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(TestBlog.Name);
            dbBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            dbBlog.NanoId.ShouldBe(TestBlog.NanoId);

            dbBlog.Name = "Updated Blog Name";

            blogRepository
                .SmartSave(dbBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to update test blog: {r.GetMessages()}"));

            var dbBlogAfterUpdate = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlogAfterUpdate.ShouldNotBeNull();
            dbBlogAfterUpdate.Name.ShouldBe("Updated Blog Name");
            dbBlogAfterUpdate.DateModified.ShouldNotBeNull();
        }

        [Fact]
        public void Update_WhenNameExceeds255Characters_ShouldFail()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole(TestRole);
            this.CreateTestUser(TestUser, TestRole);

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blog.ShouldNotBeNull();
            blog.Name.ShouldBe(TestBlog.Name);
            blog.DateCreated.ShouldBe(TestBlog.DateCreated);
            blog.NanoId.ShouldBe(TestBlog.NanoId);

            blog.Name = "Updated Blog Name " + new string('a', 500);

            var result = blogRepository.SmartSave(blog.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(2);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Name cannot exceed 255 characters");
            result.Messages[1].Message.ShouldBe("Slug cannot exceed 255 characters");

            var dbBlogAfterUpdate = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlogAfterUpdate.ShouldNotBeNull();
            dbBlogAfterUpdate.Name.ShouldBe(TestBlog.Name);
            dbBlogAfterUpdate.DateModified.ShouldBeNull();
        }
    }
}
