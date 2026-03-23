using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISuperRepository :
        ICategories<OrigamiCategory, OrigamiContent>
    {
        /// <summary>
        /// Is Origami in maintenance lock-out mode?
        /// </summary>
        bool MaintenanceLockout { get; }

        IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }

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
        IDirectoryRepository Directories { get; }
        IEmailRepository Emails { get; }
        IFileRepository Files { get; }
        IPageRepository Pages { get; }
        IPhysicalPageRepository PhysicalPages { get; }
        IPhysicalPageViewRepository PhysicalPageViews { get; }
        IPostRepository Posts { get; }
        IQuickNoteRepository QuickNotes { get; }
        IResumeRepository Resumes { get; }
        IRightRepository Rights { get; }
        IRoleRepository Roles { get; }
        ISettingsRepository Settings { get; }
        ISocialProfileRepository SocialProfiles { get; }
        ISpecialMessageRepository SpecialMessages { get; }
        ISpecialPageRepository SpecialPages { get; }
        ISubscriberRepository Subscribers { get; }
        ITagRepository Tags { get; }
        ITrashRepository Trashes { get; }
        IUserActivityRepository UserActivities { get; }
        IUserBlogRepository UserBlogs { get; }
        IUserContentRepository UserContents { get; }
        IUserPasswordResetRepository UserPasswordResets { get; }
        IUserRepository Users { get; }
        IUserRoleRepository UserRoles { get; }
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

        /// <summary>
        /// Gets all published maintenance pages
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrigamiSpecialPage> GetMaintenancePages();

        /// <summary>
        /// Get active pages
        /// </summary>
        /// <returns>non-deleted and published pages, sorted by title</returns>
        IEnumerable<OrigamiPage> GetPages();

        /// <summary>
        /// Retrieves a collection of posts associated with the specified tag.
        /// </summary>
        /// <param name="tag">The tag used to filter posts. Must not be <see langword="null"/>.</param>
        /// <returns>An enumerable collection of <see cref="OrigamiPost"/> objects that are associated with the specified tag. If
        /// no posts match the tag, the collection will be empty.</returns>
        IEnumerable<OrigamiPost> GetPosts(OrigamiTag tag);

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
        /// Gets a subscriber by its social profile, when the subscriber is not deleted and verified.
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <returns></returns>
        OrigamiSubscriber? GetSubscriber(OrigamiSocialProfile socialProfile);

        /// <summary>
        /// Retrieves a collection of videos associated with the specified tag.
        /// </summary>
        /// <param name="tag">The tag used to filter videos. Only videos that are associated with this tag are returned.</param>
        /// <returns>An enumerable collection of videos that are associated with the specified tag. The collection is empty if no
        /// videos match the tag.</returns>
        IEnumerable<OrigamiVideo> GetVideos(OrigamiTag tag);

        /// <summary>
        /// Returns all Videos associated with a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        IEnumerable<OrigamiVideo> GetVideos(OrigamiCategory category);

        object? GuessWho(string text);

        bool IsParentDeleted(OrigamiCategory category);
        bool IsParentDeleted(OrigamiContentComment comment);
        bool IsParentDeleted(OrigamiContent content);

        Result RefreshAllRepositories();
        Result RefreshAllSearchIndexes();
        Result RegenerateNanoIds();
    }
}
