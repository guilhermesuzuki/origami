using NanoidDotNet;
using Origami.Core;
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

        public static OrigamiContentComment Comment = new()
        {
            Content = "<p>Hey, this is a comment!</p>",
            ContentId = ContentId,
            DateCreated = DateTime.UtcNow,
            Ip = "192.168.0.1",
            IsApproved = true,
            IsBot = true,
            IsDeleted = false,
            IsMobileDevice = false,
            IsSpam = false,
            SocialProfileId = TestFacebookId,
        };

        public static Guid ContentId = new Guid("2e288535-0168-4625-bb4a-2e5d95e24a3b");
        public static Guid PageIdA = new Guid("223fef1e-16b7-4a98-9eba-3d489ca5abd3");
        public static Guid PageIdB = new Guid("22b9cd98-ce63-45b8-b2ed-a1cf682967dc");
        public static Guid PageIdC = new Guid("2d5ee8f2-c730-4c79-8024-cbdb786405f3");

        public static Guid PostIdA = new Guid("01ce226b-ca04-4642-a85b-5f0229961058");
        public static Guid PostIdB = new Guid("bcaf0a73-7f21-4d34-8f9c-26d2bc3e0aeb");
        public static Guid PostIdC = new Guid("a9e421de-e438-4d65-98e4-764d339a59ae");

        public static Guid RoleId = new Guid("e1f2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6a");
        public static Guid RoleId1 = new Guid("f1e2d3c4-b5a6-7d8e-9f0a-1b2c3d4e5f6b");
        public static Guid TestFacebookId = new Guid("5d89f755-65f9-44da-8b2c-8a2a390cec5d");
        public static Guid UserId = new Guid("d2c9e5b8-1c3a-4f8e-9a1b-2f3e4d5c6a7b");
        public static Guid UserRoleId = new Guid("e47c0ae6-a1c4-4631-822d-569ffe583977");

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

        public OrigamiSocialProfile TestFacebookProfile = new OrigamiSocialProfile
        {
            Id = TestFacebookId,
            SocialNetwork = SocialNetworks.Facebook,
            UserId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 25),
            Email = "123@mail.facebook.com",
            EmailFromSocialNetwork = "123@facebook.com",
            IsBlocked = false,
            IsModerator = true,
            Name = "Test facebook social profile",
            ProfilePage = "https://www.facebook.com/testprofile",
            ProfilePictureUrl = "https://www.facebook.com/images/fb_icon_325x325.png",
        };

        public OrigamiSocialProfile TestFacebookProfileButUserIsBlocked = new OrigamiSocialProfile
        {
            Id = TestFacebookId,
            SocialNetwork = SocialNetworks.Facebook,
            UserId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 25),
            Email = "123@mail.facebook.com",
            EmailFromSocialNetwork = "123@facebook.com",
            IsBlocked = true,
            IsModerator = true,
            Name = "Test facebook social profile",
            ProfilePage = "https://www.facebook.com/testprofile",
            ProfilePictureUrl = "https://www.facebook.com/images/fb_icon_325x325.png",
        };

        public OrigamiPage TestPageA = new OrigamiPage
        {
            AuthorId = UserId,
            BlogId = BlogId,
            Id = PageIdA,
            Title = "Test page A",
            Content = "<p>Hey, this is a test page A!</p>",
            DateCreated = DateTime.UtcNow
        };

        public OrigamiPage TestPageB = new OrigamiPage
        {
            AuthorId = UserId,
            BlogId = BlogId,
            Id = PageIdB,
            Title = "Test page B",
            Content = "<p>Hey, this is a test page B!</p>",
            DateCreated = DateTime.UtcNow,
            ParentId = PageIdA,
        };

        public OrigamiPage TestPageC = new OrigamiPage
        {
            AuthorId = UserId,
            BlogId = BlogId,
            Id = PageIdC,
            Title = "Test page C",
            Content = "<p>Hey, this is a test page C!</p>",
            DateCreated = DateTime.UtcNow,
            ParentId = PageIdB,
        };

        public OrigamiPost TestPostA = new OrigamiPost
        {
            AuthorId = UserId,
            BlogId = BlogId,
            Id = PostIdA,
            Title = "Test post A",
            Content = "<p>Hey, this is a test post A!</p>",
            DateCreated = DateTime.UtcNow
        };

        public OrigamiPost TestPostB = new OrigamiPost
        {
            AuthorId = UserId,
            BlogId = BlogId,
            Id = PostIdB,
            Title = "Test post B",
            Content = "<p>Hey, this is a test post B!</p>",
            DateCreated = DateTime.UtcNow,
            ParentId = PostIdA,
        };

        public OrigamiPost TestPostC = new OrigamiPost
        {
            AuthorId = UserId,
            BlogId = BlogId,
            Id = PostIdC,
            Title = "Test post C",
            Content = "<p>Hey, this is a test post C!</p>",
            DateCreated = DateTime.UtcNow,
            ParentId = PostIdB,
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
            DeleteOtherUsersPages = true,
            DeleteOtherUsersPosts = true,
            DeleteOtherUsersQuickNotes = true,
            DeleteOtherUsersSpecialMessages = true,
            DeleteOtherUsersSpecialPages = true,
            DeleteOtherUsersVideos = true,
            DeleteOwnPages = true,
            DeleteOwnPosts = true,
            DeleteOwnQuickNotes = true,
            DeleteOwnSpecialMessages = true,
            DeleteOwnSpecialPages = true,
            DeleteOwnVideos = true,
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
            PublishOwnPages = true,
            PublishOwnPosts = true,
            PublishOwnQuickNotes = true,
            PublishOwnSpecialMessages = true,
            PublishOwnSpecialPages = true,
            PublishOwnVideos = true,
            PublishOtherUsersPages = true,
            PublishOtherUsersPosts = true,
            PublishOtherUsersQuickNotes = true,
            PublishOtherUsersSpecialMessages = true,
            PublishOtherUsersSpecialPages = true,
            PublishOtherUsersVideos = true,
            PurgeBlogs = true,
            PurgeCategories = true,
            PurgeRoles = true,
            PurgeUsers = true,
            PurgePages = true,
            PurgePosts = true,
            PurgeQuickNotes = true,
            PurgeSpecialMessages = true,
            PurgeSpecialPages = true,
            PurgeTags = true,
            PurgeVideos = true,
            ResetOtherUsers2FA = true,
            ResetOtherUsersPasswords = true,
            ResetOwn2FA = true,
            ResetOwnPassword = true,
            RestoreBlogs = true,
            RestoreCategories = true,
            RestoreComments = true,
            RestorePages = true,
            RestorePosts = true,
            RestoreQuickNotes = true,
            RestoreRoles = true,
            RestoreSpecialMessages = true,
            RestoreSpecialPages = true,
            RestoreSystem = true,
            RestoreUsers = true,
            RestoreVideos = true,
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
