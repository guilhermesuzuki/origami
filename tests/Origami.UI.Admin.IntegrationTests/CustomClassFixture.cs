using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.UI.Admin.IntegrationTests
{
    public class CustomClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        public Guid BlogId = new Guid("b6af1155-5c2a-4fb5-ae64-fd1f1f19b1de");
        public Guid UserId = new Guid("d2c9e5b8-1c3a-4f8e-9a1b-2f3e4d5c6a7b");
        protected readonly CustomWebApplicationFactory _factory;
        protected readonly IServiceScope _scope;

        public CustomClassFixture(CustomWebApplicationFactory factory) : base()
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
            this.PopulateDatabase();
        }

        public void AddPermission(Guid userId, string roleName, string permissionName)
        {
            var role = _scope.ServiceProvider.GetService<IRoleRepository>()!
                .ReadFromCache()
                .FirstOrDefault(x => x.Name.Like(roleName));
                
            if (role == null)
            {
                role = new OrigamiRole { Name = roleName };

                role.GetType().GetProperty(permissionName)!.SetValue(role, true);

                _scope.ServiceProvider.GetService<IRoleRepository>()!
                    .SmartSave(role.GetContext(this.GetTestUser()), false)
                    .OnFailure(() => throw new Exception($"Failed to save role {role.Name}"));
            }

            var testRole = _scope.ServiceProvider.GetService<IUserRoleRepository>()!
                .ReadFromCache()
                .FirstOrDefault(x => x.UserId == userId && x.RoleId == role.Id);

            if (testRole == null)
            {
                testRole = new OrigamiUserRole { RoleId = role.Id, UserId = userId };

                _scope.ServiceProvider.GetService<IUserRoleRepository>()!
                    .SmartSave(testRole.GetContext(this.GetTestUser()), false)
                    .OnFailure(() => throw new Exception("Failed to save user-role relationship"));
            }
        }

        public OrigamiBlog CreateBlog()
        {
            var blog = new OrigamiBlog
            {
                Id = this.BlogId,
                Name = "Test blog",
                DateCreated = DateTime.UtcNow,
                NanoId = this.BlogId.ToString().Substring(0, 8),
            };

            _scope.ServiceProvider.GetService<IBlogRepository>()!
                .SmartSave(blog.GetContext(this.GetTestUser()), true)
                .OnFailure(() => throw new Exception($"Failed to save blog {blog.Name}"));

            return blog;
        }

        public IEnumerable<OrigamiRight> CreateRights()
        {
            var list = new List<OrigamiRight>();

            foreach (var permission in typeof(OrigamiRole).GetProperties().Where(x => x.PropertyType == typeof(bool)))
            {
                if (permission.CanRead && permission.CanWrite)
                {
                    var right = new OrigamiRight
                    {
                        Id = Guid.NewGuid(),
                        Name = permission.Name,
                    };

                    _scope.ServiceProvider.GetService<IRightRepository>()!
                        .SmartSave(right.GetContext(this.GetTestUser()), false)
                        .OnFailure(() => throw new Exception($"Failed to save right {right.Name}"));
                    
                    list.Add(right);
                }
            }

            return list;
        }

        public OrigamiUser CreateUser()
        {
            var user = new OrigamiUser
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

            _scope.ServiceProvider.GetService<IUserRepository>()!
                .SmartSave(user.GetContext(), false)
                .OnFailure(() => throw new Exception("Failed to save user"));

            this.AddPermission(UserId, "Test role", nameof(OrigamiRole.CreateNewBlogs));
            this.AddPermission(UserId, "Test role", nameof(OrigamiRole.ViewBlogs));
            this.AddPermission(UserId, "Test role", nameof(OrigamiRole.EditBlogs));
            this.AddPermission(UserId, "Test role", nameof(OrigamiRole.DeleteBlogs));

            return user;
        }

        public OrigamiUserBlog CreateUserBlog(Guid blogId, Guid userId)
        {
            var userBlog = new OrigamiUserBlog
            {
                UserId = userId,
                BlogId = blogId,
            };
            _scope.ServiceProvider.GetService<IUserBlogRepository>()!.SmartSave(userBlog.GetContext(this.GetTestUser()), false);
            return userBlog;
        }

        public OrigamiUser GetTestUser()
        {
            return _scope.ServiceProvider.GetService<IUserRepository>()!.ReadFromCache().Id(this.UserId)!;
        }

        public void PopulateDatabase()
        {
            this.CreateRights();
            this.CreateUser();
            this.CreateBlog();
            this.CreateUserBlog(this.BlogId, this.UserId);
        }
    }
}
