using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Shouldly;

namespace Origami.UI.Admin.IntegrationTests
{
    public static class Extensions
    {
        public static void CreateTestBlog(this IServiceScope scope, OrigamiBlog blog, OrigamiRole role, OrigamiUser user)
        {
            scope.CreateTestRole(role);
            scope.CreateTestUser(user, role);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;
            var superRepository = scope.ServiceProvider.GetService<ISuperRepository>()!;

            blogRepository
                .SmartSave(blog.GetContext(user), true)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));

            using var db = blogRepository.DbContextFactory.CreateDbContext();
            var dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == blog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(blog.Name);
            dbBlog.DateCreated.ShouldBe(blog.DateCreated);
            dbBlog.NanoId.ShouldBe(blog.NanoId);
            dbBlog.IsActive.ShouldBe(blog.IsActive);
            dbBlog.IsDeleted.ShouldBe(blog.IsDeleted);
            dbBlog.IsPrimary.ShouldBe(blog.IsPrimary);
            dbBlog.Version.ShouldBe(blog.Version);

            var cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(blog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Name.ShouldBe(blog.Name);
            cacheBlog.DateCreated.ShouldBe(blog.DateCreated);
            cacheBlog.NanoId.ShouldBe(blog.NanoId);
            cacheBlog.IsActive.ShouldBe(blog.IsActive);
            cacheBlog.IsDeleted.ShouldBe(blog.IsDeleted);
            cacheBlog.IsPrimary.ShouldBe(blog.IsPrimary);
            cacheBlog.Version.ShouldBe(blog.Version);

            blogRepository.Activate(blog.GetContext(user), true)
                .OnFailure(r => throw new Exception($"Failed to activate test blog: {r.GetMessages()}"));

            dbBlog = db.Blogs.AsNoTracking().FirstOrDefault(b => b.Id == blog.Id);

            dbBlog.ShouldNotBeNull();
            dbBlog.Name.ShouldBe(blog.Name);
            dbBlog.DateCreated.ShouldBe(blog.DateCreated);
            dbBlog.NanoId.ShouldBe(blog.NanoId);
            dbBlog.IsActive.ShouldBe(blog.IsActive);
            dbBlog.IsDeleted.ShouldBe(blog.IsDeleted);
            dbBlog.IsPrimary.ShouldBe(blog.IsPrimary);
            dbBlog.Version.ShouldBe(blog.Version);

            cacheBlog = superRepository.MyMemoryCache.Read<OrigamiBlog>().Id(blog.Id);
            cacheBlog.ShouldNotBeNull();
            cacheBlog.Name.ShouldBe(blog.Name);
            cacheBlog.DateCreated.ShouldBe(blog.DateCreated);
            cacheBlog.NanoId.ShouldBe(blog.NanoId);
            cacheBlog.IsActive.ShouldBe(blog.IsActive);
            cacheBlog.IsDeleted.ShouldBe(blog.IsDeleted);
            cacheBlog.IsPrimary.ShouldBe(blog.IsPrimary);
            cacheBlog.Version.ShouldBe(blog.Version);
        }

        public static void CreateTestCategory(this IServiceScope scope, OrigamiCategory category, OrigamiUser user)
        {
            var categoryRepository = scope.ServiceProvider.GetService<ICategoryRepository>()!;
            if (categoryRepository.ReadFromDatabase(category) == null)
            {
                categoryRepository
                    .SmartSave(category.GetContext(user), true)
                    .OnFailure(r => throw new Exception($"Failed to create test category: {r.GetMessages()}"));
            }
        }

        public static void CreateTestUser(this IServiceScope scope, OrigamiUser user, OrigamiRole role)
        {
            var userRepository = scope.ServiceProvider.GetService<IUserRepository>()!;
            if (userRepository.ReadFromDatabase(user) == null)
            {
                userRepository
                    .SmartSave(user.GetContext(), false)
                    .OnFailure(r => throw new Exception($"Failed to create test user: {r.GetMessages()}"));
            }

            var userRoleRepository = scope.ServiceProvider.GetService<IUserRoleRepository>()!;

            userRoleRepository
                .SmartSave(new OrigamiUserRole { UserId = user.Id, RoleId = role.Id }.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user role: {r.GetMessages()}"));
        }

        public static void CreateTestRole(this IServiceScope scope, OrigamiRole role)
        {
            var roleRepository = scope.ServiceProvider.GetService<IRoleRepository>()!;
            if (roleRepository.ReadFromDatabase(role) == null)
            {
                roleRepository
                    .SmartSave(role.GetContext(), false)
                    .OnFailure(r => throw new Exception($"Failed to create test role: {r.GetMessages()}"));
            }
        }
    }
}
