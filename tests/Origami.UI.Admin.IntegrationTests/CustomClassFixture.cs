using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI.Admin.IntegrationTests
{
    public class CustomClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        public static Guid BlogId = new Guid("b6af1155-5c2a-4fb5-ae64-fd1f1f19b1de");
        public static Guid UserId = new Guid("d2c9e5b8-1c3a-4f8e-9a1b-2f3e4d5c6a7b");
        public static Guid RoleId = new Guid("e1f2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6a");

        public OrigamiBlog TestBlog = new OrigamiBlog
        {
            Id = BlogId,
            Name = "Test blog",
            DateCreated = DateTime.UtcNow,
            NanoId = BlogId.ToString().Substring(0, 8),
        };

        public OrigamiRole TestRole = new() 
        { 
            Id = RoleId,
            Name = "Test role",
            DateCreated = DateTime.UtcNow,
            CreateNewBlogs = true,
            EditBlogs = true,
            DeleteBlogs = true,
            NanoId = RoleId.ToString().Substring(0, 8),
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

        protected void CreateTestUser()
        {
            var userRepository = _scope.ServiceProvider.GetService<IUserRepository>()!;
            userRepository
                .SmartSave(TestUser.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user: {r.GetMessages()}"));

            var userRoleRepository = _scope.ServiceProvider.GetService<IUserRoleRepository>()!;
            userRoleRepository
                .SmartSave(new OrigamiUserRole { UserId = UserId, RoleId = RoleId }.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test user role: {r.GetMessages()}"));
        }

        protected void CreateTestRole()
        {
            var roleRepository = _scope.ServiceProvider.GetService<IRoleRepository>()!;
            roleRepository
                .SmartSave(TestRole.GetContext(), false)
                .OnFailure(r => throw new Exception($"Failed to create test role: {r.GetMessages()}"));
        }
    }
}
