using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
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
        public void Delete_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();
            this.CreateTestRole();
            this.CreateTestUser();
            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;
            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));
            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);
            dbBlog.ShouldNotBeNull();
            blogRepository
                .SmartDelete(dbBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to delete test blog: {r.GetMessages()}"));
            using var dbAfterDelete = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlogAfterDelete = dbAfterDelete.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);
            dbBlogAfterDelete.ShouldNotBeNull();
            dbBlogAfterDelete.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole();
            this.CreateTestUser();

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(TestBlog.Name);
            dbBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            dbBlog.NanoId.ShouldBe(TestBlog.NanoId);
        }

        [Fact]
        public void Update_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole();
            this.CreateTestUser();

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(TestBlog.Name);
            dbBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            dbBlog.NanoId.ShouldBe(TestBlog.NanoId);

            dbBlog.Name = "Updated Blog Name";

            blogRepository
                .SmartSave(dbBlog.GetContext(TestUser), true)
                .OnFailure(r => throw new Exception($"Failed to update test blog: {r.GetMessages()}"));

            using var dbAfterUpdate = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlogAfterUpdate = dbAfterUpdate.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlogAfterUpdate.ShouldNotBeNull();
            dbBlogAfterUpdate.Name.ShouldBe("Updated Blog Name");
            dbBlogAfterUpdate.DateModified.ShouldNotBeNull();
        }
    }
}
