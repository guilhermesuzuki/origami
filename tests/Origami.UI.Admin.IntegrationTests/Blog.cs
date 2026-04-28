using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Shouldly;
using System.Transactions;

namespace Origami.UI.Admin.IntegrationTests
{
    public class Blog: CustomClassFixture
    {
        public Blog(CustomWebApplicationFactory factory) : base(factory)
        {

        }

        [Fact]
        public void Insert_WhenEntityIsValid_ShouldPersistRecord()
        {
            using var transaction = new TransactionScope();

            this.CreateTestRole();
            this.CreateTestUser();

            var blogRepository = _scope.ServiceProvider.GetService<IBlogRepository>()!;

            blogRepository
                .SmartSave(TestBlog.GetContext(TestUser), false)
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
                .SmartSave(TestBlog.GetContext(TestUser), false)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(TestBlog.Name);
            dbBlog.DateCreated.ShouldBe(TestBlog.DateCreated);
            dbBlog.NanoId.ShouldBe(TestBlog.NanoId);

            dbBlog.Name = "Updated Blog Name";

            blogRepository
                .SmartSave(dbBlog.GetContext(TestUser), false)
                .OnFailure(r => throw new Exception($"Failed to update test blog: {r.GetMessages()}"));

            using var dbAfterUpdate = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlogAfterUpdate = dbAfterUpdate.Blogs.FirstOrDefault(b => b.Id == TestBlog.Id);

            dbBlogAfterUpdate.ShouldNotBeNull();
            dbBlogAfterUpdate.Name.ShouldBe("Updated Blog Name");
            dbBlogAfterUpdate.DateModified.ShouldNotBeNull();
        }
    }
}
