using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI.Admin.IntegrationTests
{
    public static class Extensions
    {
        public static void CreateTestBlog(this IServiceScope scope, OrigamiBlog blog, OrigamiRole role, OrigamiUser user)
        {
            scope.CreateTestRole(role);
            scope.CreateTestUser(user, role);

            var blogRepository = scope.ServiceProvider.GetService<IBlogRepository>()!;
            if (blogRepository.ReadFromDatabase(blog) == null)
            {
                blogRepository
                    .SmartSave(blog.GetContext(), false)
                    .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));
            }
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
            if (userRoleRepository.ReadFromDatabase(new OrigamiUserRole { Id = CustomClassFixture.UserRoleId }) == null)
            {
                userRoleRepository
                    .SmartSave(new OrigamiUserRole { Id = CustomClassFixture.UserRoleId, UserId = user.Id, RoleId = role.Id }.GetContext(), false)
                    .OnFailure(r => throw new Exception($"Failed to create test user role: {r.GetMessages()}"));
            }
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
