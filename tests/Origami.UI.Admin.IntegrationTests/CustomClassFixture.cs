using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI.Admin.IntegrationTests
{
    public class CustomClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        public static Guid BlogId = new Guid("b6af1155-5c2a-4fb5-ae64-fd1f1f19b1de");
        public static Guid BlogId1 = new Guid("405bac29-0d05-49a2-b368-d811553e6e6f");
        public static Guid UserId = new Guid("d2c9e5b8-1c3a-4f8e-9a1b-2f3e4d5c6a7b");
        public static Guid RoleId = new Guid("e1f2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6a");
        public static Guid RoleId1 = new Guid("f1e2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6b");

        public OrigamiBlog TestBlog = new OrigamiBlog
        {
            Id = BlogId,
            Name = "Test blog",
            DateCreated = DateTime.UtcNow,
            NanoId = BlogId.ToString().Substring(0, 8),
        };

        public OrigamiBlog TestBlogWithBigName = new OrigamiBlog
        {
            Id = BlogId1,
            Name = new string('a', 500),
            DateCreated = DateTime.UtcNow,
            NanoId = BlogId1.ToString().Substring(0, 8),
        };

        public OrigamiRole TestRole = new() 
        { 
            Id = RoleId,
            DateCreated = DateTime.UtcNow,
            Name = "Test role",
            NanoId = RoleId.ToString().Substring(0, 8),
            ActivateBlogs = true,
            CreateNewBlogs = true,
            DeactivateBlogs = true,
            DeleteBlogs = true,
            EditBlogs = true,
            MarkBlogAsPrimary = true,
        };

        public OrigamiRole TestRoleNoPermissions = new()
        {
            Id = RoleId1,
            DateCreated = DateTime.UtcNow,
            Name = "Test role (no permissions)",
            NanoId = RoleId1.ToString().Substring(0, 8),
        };

        public OrigamiUser TestUser = new OrigamiUser
        {
            Id = UserId,
            DateCreated = DateTime.UtcNow,
            DisplayName = "Test user",
            EmailAddress = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            NanoId = UserId.ToString().Substring(0, 8),
            Password = "123test@test".SHA256Hash(),
            Username = "testuser",
        };

        protected readonly CustomWebApplicationFactory _factory;
        protected readonly IServiceScope _scope;

        public CustomClassFixture(CustomWebApplicationFactory factory) : base()
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
        }

        protected void CreateTestUser(OrigamiUser user, OrigamiRole role)
        {
            var userRepository = _scope.ServiceProvider.GetService<IUserRepository>()!;
            userRepository
                .SmartSave(user.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user: {r.GetMessages()}"));

            var userRoleRepository = _scope.ServiceProvider.GetService<IUserRoleRepository>()!;
            userRoleRepository
                .SmartSave(new OrigamiUserRole { UserId = user.Id, RoleId = role.Id }.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user role: {r.GetMessages()}"));
        }

        protected void CreateTestRole(OrigamiRole role)
        {
            var roleRepository = _scope.ServiceProvider.GetService<IRoleRepository>()!;
            roleRepository
                .SmartSave(role.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test role: {r.GetMessages()}"));
        }

        protected void CreateTestRoleNoPermissions()
        {
            var roleRepository = _scope.ServiceProvider.GetService<IRoleRepository>()!;
            roleRepository
                .SmartSave(TestRoleNoPermissions.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test role (no permissions): {r.GetMessages()}"));
        }
    }
}
