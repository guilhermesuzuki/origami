using Microsoft.Extensions.DependencyInjection;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Origami.UI.Admin.IntegrationTests
{
    public class CustomClassFixture : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        public static Guid BlogId = new Guid("b6af1155-5c2a-4fb5-ae64-fd1f1f19b1de");
        public static Guid BlogId1 = new Guid("405bac29-0d05-49a2-b368-d811553e6e6f");
        public static Guid CategoryId = new Guid("c1d2e3f4-5a6b-7c8d-9e0f-1a2b3c4d5e6f");
        public static Guid CategoryId1 = new Guid("f8358f3e-073d-44f9-bd12-a87b8af2bd31");
        public static Guid CategoryIdA = new Guid("1824ed20-0d55-476c-a716-c531667aa8ce");
        public static Guid CategoryIdB = new Guid("27e926c3-fc50-484d-85fb-328ff7f51b82");
        public static Guid CategoryIdC = new Guid("111bd28d-04a9-4e9a-b6c7-436e43aba651");
        public static Guid RoleId = new Guid("e1f2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6a");
        public static Guid RoleId1 = new Guid("f1e2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6b");
        public static Guid UserId = new Guid("d2c9e5b8-1c3a-4f8e-9a1b-2f3e4d5c6a7b");

        public OrigamiUser AnotherTestUser = new OrigamiUser
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            DisplayName = "Another test user",
            EmailAddress = "another@testuser.com",
            FirstName = "Another",
            LastName = "Test user",
            Username = "another_test_user",
            NanoId = Guid.NewGuid().ToString().Substring(0, 8),
            IsBlocked = false,
            IsDeleted = false,
        };

        public OrigamiBlog TestBlog = new OrigamiBlog
        {
            Id = BlogId,
            Name = "Test blog",
            DateCreated = DateTime.UtcNow,
            NanoId = BlogId.ToString().Substring(0, 8),
            IsActive = false,
            IsDeleted = false,
        };

        public OrigamiBlog TestBlogWithBigName = new OrigamiBlog
        {
            Id = BlogId1,
            Name = new string('a', 500),
            DateCreated = DateTime.UtcNow,
            NanoId = BlogId1.ToString().Substring(0, 8),
        };

        public OrigamiCategory TestCategory = new OrigamiCategory
        {
            BlogId = BlogId,
            Id = CategoryId,
            Name = "Test category",
            DateCreated = DateTime.UtcNow,
            NanoId = CategoryId.ToString().Substring(0, 8),
        };

        public OrigamiCategory TestCategoryA = new OrigamiCategory
        {
            BlogId = BlogId,
            Id = CategoryIdA,
            Name = "Test category A",
            DateCreated = DateTime.UtcNow,
            NanoId = CategoryIdA.ToString().Substring(0, 8),
        };

        public OrigamiCategory TestCategoryB = new OrigamiCategory
        {
            BlogId = BlogId,
            ParentId = CategoryIdA,
            Id = CategoryIdB,
            Name = "Test category B",
            DateCreated = DateTime.UtcNow,
            NanoId = CategoryIdB.ToString().Substring(0, 8),
        };

        public OrigamiCategory TestCategoryC = new OrigamiCategory
        {
            BlogId = BlogId,
            ParentId = CategoryIdB,
            Id = CategoryIdC,
            Name = "Test category C",
            DateCreated = DateTime.UtcNow,
            NanoId = CategoryIdC.ToString().Substring(0, 8),
        };

        public OrigamiCategory TestCategoryWithBigName = new OrigamiCategory
        {
            BlogId = BlogId,
            Id = CategoryId1,
            Name = new string('a', 500),
            DateCreated = DateTime.UtcNow,
            NanoId = CategoryId1.ToString().Substring(0, 8),
        };

        public OrigamiRole TestRole = new() 
        { 
            Id = RoleId,
            DateCreated = DateTime.UtcNow,
            Name = "Test role",
            NanoId = RoleId.ToString().Substring(0, 8),
            ActivateBlogs = true,
            BlockUserSelf = true,
            BlockUsersOtherThanSelf = true,
            CreateNewBlogs = true,
            CreateNewCategories = true,
            CreateNewPages = true,
            CreateNewPosts = true,
            CreateNewQuickNotes = true,
            CreateNewRoles = true,
            CreateNewSpecialMessages = true,
            CreateNewSpecialPages = true,
            CreateNewUsers = true,
            CreateNewVideos = true,
            DeactivateBlogs = true,
            DeleteBlogs = true,
            DeleteCategories = true,
            DeleteRoles = true,
            DeleteUserSelf = true,
            DeleteUsersOtherThanSelf = true,
            EditBlogs = true,
            EditCategories = true,
            EditOtherUsers = true,
            EditOtherUsersPages = true,
            EditOtherUsersPosts = true,
            EditOtherUsersQuickNotes = true,
            EditOtherUsersRoles = true,
            EditOtherUsersSpecialMessages = true,
            EditOtherUsersSpecialPages = true,
            EditOwnPages = true,
            EditOwnPosts = true,
            EditOwnQuickNotes = true,
            EditOwnSpecialMessages = true,
            EditOwnSpecialPages = true,
            EditOwnUser = true,
            EditOwnVideos = true,
            EditRoles = true,
            MarkBlogAsPrimary = true,
            PurgeBlogs = true,
            PurgeCategories = true,
            PurgeRoles = true,
            PurgeUsers = true,
            ResetOtherUsers2FA = true,
            ResetOtherUsersPasswords = true,
            ResetOwn2FA = true,
            ResetOwnPassword = true,
            RestoreBlogs = true,
            RestoreCategories = true,
            RestoreRoles = true,
            RestoreUsers = true,
            UnblockUsers = true,
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

        public CustomClassFixture() : base()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
