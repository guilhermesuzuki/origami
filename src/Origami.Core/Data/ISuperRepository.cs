using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISuperRepository :
        ICategories<OrigamiCategory, OrigamiContent>
    {
        IAppFacade AppFacade { get; }

        IBackupRestoreRepository BackupAndRestores { get; }

        IBlogRepository Blogs { get; }

        ICategoryRepository Categories { get; }

        IConfiguration Configurations { get; }

        IContentCategoryRepository ContentCategories { get; }

        IContentCommentReactionRepository ContentCommentReactions { get; }

        IContentCommentRepository ContentComments { get; }

        IContentHistoryRepository ContentHistories { get; }

        IContentRatingRepository ContentRatings { get; }

        IContentReactionRepository ContentReactions { get; }

        IContentRepository Contents { get; }

        IContentTagRepository ContentTags { get; }

        IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }

        IDirectoryRepository Directories { get; }

        IEmailRepository Emails { get; }

        IFileRepository Files { get; }

        /// <summary>
        /// Is Origami in maintenance lock-out mode?
        /// </summary>
        bool MaintenanceLockout { get; }

        IMyMemoryCache MyMemoryCache { get; }
        IPageRepository Pages { get; }
        IPhysicalPageRepository PhysicalPages { get; }
        IPhysicalPageViewRepository PhysicalPageViews { get; }
        IPostRepository Posts { get; }
        IRightRepository Rights { get; }
        IRoleRepository Roles { get; }
        ISettingsRepository Settings { get; }
        ISocialProfileRepository SocialProfiles { get; }
        ISpecialMessageRepository SpecialMessages { get; }
        ISpecialPageRepository SpecialPages { get; }
        ISubscriberRepository Subscribers { get; }
        IUserActivityRepository UserActivities { get; }
        IUserBlogRepository UserBlogs { get; }
        IUserPasswordResetRepository UserPasswordResets { get; }
        IUserRoleRepository UserRoles { get; }
        IUserRepository Users { get; }
        IUserTrashRepository UserTrashes { get; }
        IUserViewRepository UserViews { get; }
        IVideoRepository Videos { get; }
        IWhatToSeeNextRepository WhatToSeeNext { get; }

        bool EmptyHome(Guid blogId);

        /// <summary>
        /// Retrieves the author associated with the specified author identifier.
        /// </summary>
        /// <param name="authorId">An object that uniquely identifies the author to retrieve. Cannot be null.</param>
        /// <returns>An instance of OrigamiUser representing the author associated with the given identifier. Returns null if no
        /// matching author is found.</returns>
        OrigamiUser GetAuthor(IAuthorId authorId);

        /// <summary>
        /// Get active categories
        /// </summary>
        /// <returns>non-deleted categories, sorted by name</returns>
        IEnumerable<OrigamiCategory> GetCategories();

        /// <summary>
        /// Gets all comments for a specific blog
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        IEnumerable<OrigamiContentComment> GetComments(Guid blogId);

        IEnumerable<OrigamiContent> GetContents(OrigamiContentTag tag, Guid blogId);
        /// <summary>
        /// Draft pages
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiPage> GetDraftPages(Guid blog);

        /// <summary>
        /// Draft posts
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiPost> GetDraftPosts(Guid blog);

        /// <summary>
        /// Draft special messages
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSpecialMessage> GetDraftSpecialMessages();

        /// <summary>
        /// Draft special pages
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSpecialPage> GetDraftSpecialPages();

        /// <summary>
        /// Draft videos
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiVideo> GetDraftVideos(Guid blog);

        /// <summary>
        /// Gets all published maintenance pages
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSpecialPage> GetMaintenancePages();

        /// <summary>
        /// Retrieves all pages associated with the specified blog.
        /// </summary>
        /// <param name="blog">The unique identifier of the blog for which to retrieve pages.</param>
        /// <returns>An enumerable collection of pages belonging to the specified blog. The collection is empty if the blog
        /// contains no pages.</returns>
        IEnumerable<OrigamiPage> GetPages(Guid blog);

        /// <summary>
        /// Retrieves all posts associated with the specified blog.
        /// </summary>
        /// <param name="blog">The unique identifier of the blog for which to retrieve posts.</param>
        /// <returns>An enumerable collection of posts belonging to the specified blog. The collection is empty if the blog
        /// contains no posts.</returns>
        IEnumerable<OrigamiPost> GetPosts(Guid blog);

        /// <summary>
        /// Returns all posts associated with a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<OrigamiPost> GetPosts(OrigamiCategory category);

        /// <summary>
        /// Retrieves a collection of pages that are related to the specified page by its type.
        /// </summary>
        /// <param name="page">The page for which related pages are to be retrieved. This parameter cannot be null.</param>
        /// <returns>An enumerable collection of <see cref="OrigamiSpecialPage"/> objects representing the related pages. If no
        /// related pages are found, the collection will be empty.</returns>
        IEnumerable<OrigamiSpecialPage> GetRelatedPages(OrigamiSpecialPage page);

        /// <summary>
        /// Get direct replies to a comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        IEnumerable<OrigamiContentComment> GetReplies(OrigamiContentComment comment);

        /// <summary>
        /// Retrieves a collection of software releases available in the system.
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSoftwareRelease> GetSoftwareReleases(Guid blog);

        /// <summary>
        /// Retrieves a collection of special messages available in the system.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="OrigamiSpecialMessage"/> objects representing the special messages.
        /// The collection will be empty if no special messages are available.</returns>
        IEnumerable<OrigamiSpecialMessage> GetSpecialMessages();

        /// <summary>
        /// Retrieves a collection of special pages available in the system.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="OrigamiSpecialPage"/> objects representing the special pages. The
        /// collection will be empty if no special pages are available.</returns>
        IEnumerable<OrigamiSpecialPage> GetSpecialPages();

        /// <summary>
        /// Gets a subscriber by its social profile, when the subscriber is not deleted and verified.
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <returns></returns>
        OrigamiSubscriber? GetSubscriber(OrigamiSocialProfile socialProfile);

        /// <summary>
        /// Retrieves the collection of tags associated with the specified content item.
        /// </summary>
        /// <param name="content">The content item for which to retrieve tags. Cannot be null.</param>
        /// <returns>An enumerable collection of tags linked to the specified content. The collection is empty if the content has
        /// no tags.</returns>
        IEnumerable<OrigamiContentTag> GetTags(OrigamiContent content);

        /// <summary>
        /// Retrieves a collection of videos associated with the specified blog.
        /// </summary>
        /// <param name="blog">The unique identifier of the blog for which to retrieve videos.</param>
        /// <returns>An enumerable collection of videos belonging to the specified blog. The collection is empty if the blog
        /// contains no videos.</returns>
        IEnumerable<OrigamiVideo> GetVideos(Guid blog);

        /// <summary>
        /// Returns all Videos associated with a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<OrigamiVideo> GetVideos(OrigamiCategory category);

        /// <summary>
        /// Get active pages
        /// </summary>
        /// <returns>non-deleted and published pages, sorted by title</returns>
        IEnumerable<OrigamiPage> GetVisiblePages();

        object? GuessWho(string text);

        bool IsParentDeleted<T>(T entity) where T : class, IId, IParentIdNull, IDeleted;

        Result RefreshAllRepositories();
        Result RefreshAllSearchIndexes();
        Result RegenerateNanoIds();
    }
}
