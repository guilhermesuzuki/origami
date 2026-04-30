using BootstrapBlazor.Components;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.UI.Admin.IntegrationTests
{
    public static class Extensions
    {
        public static void CreateTestBlog(this IServiceScope scope, OrigamiBlog blog, OrigamiRole role, OrigamiUser user)
        {
            scope.CreateTestRole(role);
            scope.CreateTestUser(user, role);
            scope.ServiceProvider.GetService<IBlogRepository>()!
                .SmartSave(blog.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test blog: {r.GetMessages()}"));
        }

        public static void CreateTestCategory(this IServiceScope scope, OrigamiCategory category, OrigamiUser user)
        {
            scope.ServiceProvider.GetService<ICategoryRepository>()!
                .SmartSave(category.GetContext(user), true)
                .OnFailure(r => throw new Exception($"Failed to create test category: {r.GetMessages()}"));
        }

        public static void CreateTestUser(this IServiceScope scope, OrigamiUser user, OrigamiRole role)
        {
            var userRepository = scope.ServiceProvider.GetService<IUserRepository>()!;
            userRepository
                .SmartSave(user.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user: {r.GetMessages()}"));

            var userRoleRepository = scope.ServiceProvider.GetService<IUserRoleRepository>()!;
            userRoleRepository
                .SmartSave(new OrigamiUserRole { UserId = user.Id, RoleId = role.Id }.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user role: {r.GetMessages()}"));
        }

        public static void CreateTestRole(this IServiceScope scope, OrigamiRole role)
        {
            var roleRepository = scope.ServiceProvider.GetService<IRoleRepository>()!;
            roleRepository
                .SmartSave(role.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test role: {r.GetMessages()}"));
        }
    }
}
