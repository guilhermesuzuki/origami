using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    /// <summary>
    /// TODO: add assertions for cache where applicable
    /// </summary>
    public class BlogTests : CustomClassFixture
    {
        public BlogTests(CustomWebApplicationFactory factory) : base(factory)
        {

        }

        [Fact]
        public void Activate_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            using var db = superRepository.DbContextFactory.CreateDbContext();
            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blog.ShouldNotBeNull();
            blog.Name.ShouldBe(TestBlog.Name);
            blog.DateCreated.ShouldBe(TestBlog.DateCreated);
            blog.NanoId.ShouldBe(TestBlog.NanoId);
            blog.IsActive.ShouldBe(false);
            blog.IsActive.ShouldBe(TestBlog.IsActive);

            blogRepository
                .Activate(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to activate test blog: {r.GetMessages()}"));

            var activatedBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            activatedBlog.ShouldNotBeNull();
            activatedBlog.IsActive.ShouldBeTrue();

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Name.ShouldBe(TestBlog.Name);
            cacheBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            cacheBlog.NanoId.ShouldBe(TestBlog.NanoId);
            cacheBlog.IsActive.ShouldBe(true);
            cacheBlog.IsActive.ShouldBe(TestBlog.IsActive);

            cacheBlog.Version.ShouldBe(TestBlog.Version);
            cacheBlog.Version.ShouldBe(activatedBlog.Version);
        }

        [Fact]
        public void Deactivate_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = superRepository.DbContextFactory.CreateDbContext();
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

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Name.ShouldBe(TestBlog.Name);
            cacheBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            cacheBlog.NanoId.ShouldBe(TestBlog.NanoId);
            cacheBlog.IsActive.ShouldBe(false);
            cacheBlog.IsActive.ShouldBe(TestBlog.IsActive);

            cacheBlog.Version.ShouldBe(TestBlog.Version);
            cacheBlog.Version.ShouldBe(deactivatedBlog.Version);
        }

        [Fact]
        public void Delete_WhenBlogIsPrimary_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

            OrigamiBlog primary = null!;

            var exception = Record.Exception(() => primary = blogRepository.GetPrimary());
            exception.ShouldBeNull();

            primary.ShouldNotBeNull();
            primary.IsPrimary.ShouldBeTrue();

            var result = blogRepository.SmartDelete(primary.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(1);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Primary blog cannot be deleted");

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(primary.Id);
            cacheBlog.ShouldNotBeNull();
        }

        [Fact]
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;
            using var db = superRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blog.ShouldNotBeNull();

            blogRepository
                .SmartDelete(blog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to delete test blog: {r.GetMessages()}"));

            var dbBlog = superRepository.Blogs.ReadFromDatabase(blog);
            dbBlog.ShouldNotBeNull();
            dbBlog.DateCreated.ShouldBe(blog.DateCreated);
            dbBlog.Name.ShouldBe(blog.Name);
            dbBlog.NanoId.ShouldBe(blog.NanoId);
            dbBlog.IsDeleted.ShouldBe(true);
            dbBlog.IsDeleted.ShouldBe(blog.IsDeleted);

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id)!;
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Name.ShouldBe(TestBlog.Name);
            cacheBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            cacheBlog.NanoId.ShouldBe(TestBlog.NanoId);
            cacheBlog.IsDeleted.ShouldBe(true);

            cacheBlog.Version.ShouldBe(blog.Version);
            cacheBlog.Version.ShouldBe(dbBlog.Version);
        }

        [Fact]
        public void GetPrimary_WhenEntityIsValid_ShouldNotThrowException()
        {
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;
            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

            OrigamiBlog primary = null!;

            var exception = Record.Exception(() => primary = blogRepository.GetPrimary());
            exception.ShouldBeNull();

            primary.ShouldNotBeNull();
            primary.IsPrimary.ShouldBeTrue();
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestBlog(TestBlog, TestRole, TestUser);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;
            using var db = superRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blog.ShouldNotBeNull();
            blog.Name.ShouldBe(TestBlog.Name);
            blog.DateCreated.ShouldBe(TestBlog.DateCreated);
            blog.NanoId.ShouldBe(TestBlog.NanoId);
            blog.IsDeleted.ShouldBe(false);
            blog.IsDeleted.ShouldBe(blog.IsDeleted);

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id)!;
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Name.ShouldBe(TestBlog.Name);
            cacheBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            cacheBlog.NanoId.ShouldBe(TestBlog.NanoId);
            cacheBlog.IsDeleted.ShouldBe(false);
            cacheBlog.IsDeleted.ShouldBe(blog.IsDeleted);

            cacheBlog.Version.ShouldBe(TestBlog.Version);
            cacheBlog.Version.ShouldBe(blog.Version);
        }

        [Fact]
        public void Insert_WhenNameIsTooLarge_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

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
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlogWithBigName.Id);
            blog.ShouldBeNull();
        }

        [Fact]
        public void Insert_WhenNoPermissions_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRoleNoPermissions);
            scope.CreateTestUser(TestUser, TestRoleNoPermissions);

            var blogRepository = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

            var exception = blogRepository.SmartSave(TestBlog.GetContext(TestUser), true);

            exception.ShouldNotBeNull();
            exception.Ok.ShouldBeFalse();
            exception.Messages.ShouldNotBeNull();
            exception.Messages.Count.ShouldBe(2);
            exception.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Info);
            exception.Messages[0].Message.ShouldBe("CreateNewBlogs");
            exception.Messages[1].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            exception.Messages[1].Message.ShouldBe("You don't have permission for this feature");

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blog.ShouldBeNull();
        }

        [Fact]
        public void Purge_WhenBlogIsPrimary_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);
            var blogRepository = scope.ServiceProvider.GetRequiredService<IBlogRepository>();

            OrigamiBlog primary = blogRepository.GetPrimary();

            primary.ShouldNotBeNull();
            primary.IsPrimary.ShouldBeTrue();

            var result = blogRepository.SmartPurge(primary.GetContext(TestUser), true);

            result.ShouldNotBeNull();
            result.Ok.ShouldBeFalse();
            result.Messages.ShouldNotBeNull();
            result.Messages.Count.ShouldBe(1);
            result.Messages[0].MessageType.ShouldBe(ResultMessage.MessageTypes.Error);
            result.Messages[0].Message.ShouldBe("Primary blog cannot be purged");
        }

        [Fact]
        public void Purge_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);
            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blog.ShouldNotBeNull();

            blogRepository
                .SmartDelete(blog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to delete test blog: {r.GetMessages()}"));

            var blogAfterDelete = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blogAfterDelete.ShouldNotBeNull();
            blogAfterDelete.IsDeleted.ShouldBeTrue();

            blogRepository
                .SmartPurge(blogAfterDelete.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to purge test blog: {r.GetMessages()}"));

            var blogAfterPurge = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blogAfterPurge.ShouldBeNull();
        }

        [Fact]
        public void Restore_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);
            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blog.ShouldNotBeNull();

            blogRepository
                .SmartDelete(blog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to delete test blog: {r.GetMessages()}"));

            var blogAfterDelete = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blogAfterDelete.ShouldNotBeNull();
            blogAfterDelete.IsDeleted.ShouldBeTrue();

            blogRepository
                .SmartRestore(blogAfterDelete.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to restore test blog: {r.GetMessages()}"));

            var blogAfterRestore = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);
            blogAfterRestore.ShouldNotBeNull();
            blogAfterRestore.IsDeleted.ShouldBeFalse();

            blogAfterRestore.Version.ShouldBe(blogAfterDelete.Version);
        }

        [Fact]
        public void SetPrimary_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

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

            newPrimary.Version.ShouldBe(TestBlog.Version);

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Version.ShouldBe(TestBlog.Version);
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var blog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blog.ShouldNotBeNull();
            blog.Name.ShouldBe(TestBlog.Name);
            blog.DateCreated.ShouldBe(TestBlog.DateCreated);
            blog.NanoId.ShouldBe(TestBlog.NanoId);

            blog.Name = "Updated Blog Name";

            blogRepository
                .SmartSave(blog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to update test blog: {r.GetMessages()}"));

            var blogAfterUpdate = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == TestBlog.Id);

            blogAfterUpdate.ShouldNotBeNull();
            blogAfterUpdate.Name.ShouldBe("Updated Blog Name");
            blogAfterUpdate.DateModified.ShouldNotBeNull();

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Version.ShouldBe(blog.Version);
            cacheBlog.Version.ShouldBe(blogAfterUpdate.Version);
        }

        [Fact]
        public void Update_WhenNameIsTooLarge_ShouldFail()
        {
            using var transaction = new TransactionScope();
            using var scope = _factory.Services.CreateScope();
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            scope.CreateTestRole(TestRole);
            scope.CreateTestUser(TestUser, TestRole);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;

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

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(TestBlog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Version.ShouldBe(blog.Version);
            cacheBlog.Version.ShouldBe(dbBlogAfterUpdate.Version);
        }
    }
}
