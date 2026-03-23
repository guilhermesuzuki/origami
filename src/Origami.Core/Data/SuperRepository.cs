using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NanoidDotNet;
using Origami.Core.Models;
using System.Globalization;
using System.Transactions;

namespace Origami.Core.Data
{
    public class SuperRepository : ISuperRepository
    {
        public SuperRepository(
            IAppFacade appFacade,
            IBackupRestoreRepository backupAndRestores,
            IBlogRepository blogRepository,
            ICategoryRepository categoryRepository,
            IConfiguration configuration,
            IDirectoryRepository directoryRepository,
            IEmailRepository emailRepository,
            IFileRepository fileRepository,
            IPageRepository pageRepository,
            IPhysicalPageRepository physicalPageRepository,
            IPhysicalPageViewRepository physicalPageViewRepository,
            IPostRepository postRepository,
            IQuickNoteRepository quickNoteRepository,
            IResumeRepository resumeRepository,
            IRightRepository rightRepository,
            IRoleRepository roleRepository,
            ISettingsRepository settingsRepository,
            ISocialProfileRepository socialProfileRepository,
            ISpecialMessageRepository specialMessageRepository,
            ISpecialPageRepository specialPageRepository,
            ISubscriberRepository subscriberRepository,
            ITagRepository tagRepository,
            ITrashRepository trashRepository,
            IUserActivityRepository userActivityRepository,
            IUserBlogRepository userBlogRepository,
            IUserContentRepository userContentRepository,
            IUserPasswordResetRepository userPasswordResetRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IUserTrashRepository userTrashRepository,
            IUserViewRepository userViewRepository,
            IVideoRepository videoRepository,
            IWhatToSeeNextRepository whatToSeeNextRepository,

            IContentCategoryRepository contentCategoryRepository,
            IContentCommentReactionRepository contentCommentReactionRepository,
            IContentCommentRepository contentCommentRepository,
            IContentHistoryRepository contentHistoryRepository,
            IContentRatingRepository contentRatingRepository,
            IContentReactionRepository contentReactionRepository,
            IContentRepository contentRepository,
            IContentTagRepository contentTagRepository,

            IDbContextFactory<OrigamiDbContext> dbContextFactory) : base()
        {
            AppFacade = appFacade;
            BackupAndRestores = backupAndRestores;
            Blogs = blogRepository;
            Categories = categoryRepository;
            Configurations = configuration;
            DbContextFactory = dbContextFactory;
            Directories = directoryRepository;
            Emails = emailRepository;
            Files = fileRepository;
            Pages = pageRepository;
            PhysicalPages = physicalPageRepository;
            PhysicalPageViews = physicalPageViewRepository;
            Posts = postRepository;
            QuickNotes = quickNoteRepository;
            Resumes = resumeRepository;
            Rights = rightRepository;
            Roles = roleRepository;
            Settings = settingsRepository;
            SocialProfiles = socialProfileRepository;
            SpecialMessages = specialMessageRepository;
            SpecialPages = specialPageRepository;
            Subscribers = subscriberRepository;
            Tags = tagRepository;
            Trashes = trashRepository;
            UserActivities = userActivityRepository;
            UserBlogs = userBlogRepository;
            UserContents = userContentRepository;
            UserPasswordResets = userPasswordResetRepository;
            UserRoles = userRoleRepository;
            Users = userRepository;
            UserTrashes = userTrashRepository;
            UserViews = userViewRepository;
            Videos = videoRepository;
            WhatToSeeNext = whatToSeeNextRepository;

            ContentCategories = contentCategoryRepository;
            ContentCommentReactions = contentCommentReactionRepository;
            ContentComments = contentCommentRepository;
            ContentHistories = contentHistoryRepository;
            ContentRatings = contentRatingRepository;
            ContentReactions = contentReactionRepository;
            Contents = contentRepository;
            ContentTags = contentTagRepository;
        }

        public IAppFacade AppFacade { get; }
        public IBackupRestoreRepository BackupAndRestores { get; }
        public IBlogRepository Blogs { get; }
        public ICategoryRepository Categories { get; }
        public IConfiguration Configurations { get; }
        public IContentCategoryRepository ContentCategories { get; }
        public IContentCommentReactionRepository ContentCommentReactions { get; }
        public IContentCommentRepository ContentComments { get; }
        public IContentHistoryRepository ContentHistories { get; }
        public IContentRatingRepository ContentRatings { get; }
        public IContentReactionRepository ContentReactions { get; }
        public IContentRepository Contents { get; }
        public IContentTagRepository ContentTags { get; }
        public IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }
        public IDirectoryRepository Directories { get; }
        public IEmailRepository Emails { get; }
        public IFileRepository Files { get; }
        public bool MaintenanceLockout => this.GetMaintenancePages().Any();
        public IPageRepository Pages { get; }
        public IPhysicalPageRepository PhysicalPages { get; }
        public IPhysicalPageViewRepository PhysicalPageViews { get; }
        public IPostRepository Posts { get; }
        public IQuickNoteRepository QuickNotes { get; }
        public IResumeRepository Resumes { get; }
        public IRightRepository Rights { get; }
        public IRoleRepository Roles { get; }
        public ISettingsRepository Settings { get; }
        public ISocialProfileRepository SocialProfiles { get; }
        public ISpecialMessageRepository SpecialMessages { get; }
        public ISpecialPageRepository SpecialPages { get; }
        public ISubscriberRepository Subscribers { get; }
        public ITagRepository Tags { get; }
        public ITrashRepository Trashes { get; }
        public IUserActivityRepository UserActivities { get; }
        public IUserBlogRepository UserBlogs { get; }
        public IUserContentRepository UserContents { get; }
        public IUserPasswordResetRepository UserPasswordResets { get; }
        public IUserRoleRepository UserRoles { get; }
        public IUserRepository Users { get; }
        public IUserTrashRepository UserTrashes { get; }
        public IUserViewRepository UserViews { get; }
        public IVideoRepository Videos { get; }
        public IWhatToSeeNextRepository WhatToSeeNext { get; }
        public bool EmptyHome(Guid blogId)
        {
            if (Pages.ReadFromCache().FrontPage(blogId) != null) return false;
            if (Posts.ReadFromCache().Published().Blog(blogId).Any() == true) return false;
            if (Videos.ReadFromCache().Published().Blog(blogId).Any() == true) return false;
            if (QuickNotes.ReadFromCache().Published().Blog(blogId).Any() == true) return false;
            return true;
        }

        public OrigamiUser GetAuthor(IAuthorId authorId)
        {
            return this.Users.ReadFromCache().Id(authorId.AuthorId) ?? new();
        }

        public IEnumerable<OrigamiCategory> GetCategories()
        {
            return from x in Categories.ReadFromCache()
                   where x.IsDeleted == false
                   where this.IsParentDeleted(x) == false
                   orderby x.Name
                   select x;
        }

        public IEnumerable<OrigamiCategory> GetCategories(OrigamiContent content)
        {
            return from a in ContentCategories.ReadFromCache()
                   join b in Categories.ReadFromCache() on a.CategoryId equals b.Id
                   where a.ContentId == content.Id
                   where b.IsDeleted == false
                   where this.IsParentDeleted(b) == false
                   orderby b.Name
                   select b;
        }

        public IEnumerable<OrigamiContentComment> GetComments(Guid blog)
        {
            return from co in ContentComments.ReadFromCache()
                   join ct in Contents.ReadFromCache() on co.ContentId equals ct.Id
                   where ct.BlogId == blog
                   select co;
        }

        public IEnumerable<OrigamiSpecialPage> GetMaintenancePages()
        {
            var pages = from x in SpecialPages.ReadFromCache()
                        where x.Type == OrigamiSpecialPageTypes.Maintenance.ToString()
                        where x.IsDeleted == false
                        where x.IsPublished == true
                        select x;

            if (pages.Any() == true)
            {
                return pages
                    .OrderBy(x => x.LanguageWrittenOn.Like(CultureInfo.CurrentUICulture.Name) ? 0 : 5)
                    .ThenBy(x => x.LanguageWrittenOn.StartsWith(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName) ? 0 : 10)
                    .ThenBy(x => x.LanguageWrittenOn);
            }

            return [];
        }

        public IEnumerable<OrigamiPage> GetPages()
        {
            return from x in Pages.ReadFromCache()
                   where x.IsDeleted == false
                   where this.IsParentDeleted(x) == false
                   where x.IsPublished == true
                   orderby x.Title
                   select x;
        }

        public IEnumerable<OrigamiPost> GetPosts(OrigamiTag tag)
        {
            return from a in ContentTags.ReadFromCache()
                   join b in Contents.ReadFromCache().OfType<OrigamiPost>() on a.ContentId equals b.Id
                   where b.BlogId == tag.BlogId
                   where a.Tag.Like(tag.Name)
                   select b;
        }

        public IEnumerable<OrigamiPost> GetPosts(OrigamiCategory category)
        {
            return from a in ContentCategories.ReadFromCache()
                   join b in Contents.ReadFromCache().OfType<OrigamiPost>() on a.ContentId equals b.Id
                   where a.CategoryId == category.Id
                   select b;
        }

        public IEnumerable<OrigamiSpecialPage> GetRelatedPages(OrigamiSpecialPage page)
        {
            return from a in SpecialPages.ReadFromCache()
                   where a.Id != page.Id
                   where a.Type == page.Type
                   select a;
        }

        public IEnumerable<OrigamiContentComment> GetReplies(OrigamiContentComment comment)
        {
            var replies = from x in ContentComments.ReadFromCache().NonDeleted()
                          where x.ParentId == comment.Id
                          where x.IsSpam == false
                          where x.IsDeleted == false
                          where x.IsApproved == true
                          orderby x.DateCreated descending
                          select x;

            return replies;
        }

        public OrigamiSubscriber? GetSubscriber(OrigamiSocialProfile socialProfile)
        {
            return Subscribers.ReadFromCache().NonDeleted()
                .Where(x => x.IsVerified == true)
                .Where(x => x.SocialProfileId == socialProfile.Id)
                .FirstOrDefault();
        }

        public IEnumerable<OrigamiContentTag> GetTags(OrigamiContent content)
        {
            return from a in ContentTags.ReadFromCache()
                   join b in Contents.ReadFromCache() on a.ContentId equals b.Id
                   where b.Id == content.Id
                   orderby a.Tag
                   select a;
        }

        public IEnumerable<OrigamiVideo> GetVideos(OrigamiTag tag)
        {
            return from a in ContentTags.ReadFromCache()
                   join b in Contents.ReadFromCache().OfType<OrigamiVideo>() on a.ContentId equals b.Id
                   where a.Tag.Like(tag.Name)
                   where b.BlogId == tag.BlogId
                   select b;
        }

        public IEnumerable<OrigamiVideo> GetVideos(OrigamiCategory category)
        {
            return from a in ContentCategories.ReadFromCache()
                   join b in Contents.ReadFromCache().OfType<OrigamiVideo>() on a.ContentId equals b.Id
                   where a.CategoryId == category.Id
                   select b;
        }

        public object? GuessWho(string text)
        {
            if (Guid.TryParse(text, out var guid) == true)
            {
                return GetIds().FirstOrDefault(x => x.Id == guid);
            }

            return GetNanoIds().FirstOrDefault(x => x.NanoId == text);
        }

        public bool IsParentDeleted(OrigamiCategory category)
        {
            if (category.ParentId.HasValue)
            {
                var parent = Categories.ReadFromCache().Id(category.ParentId.Value);
                if (parent != null)
                {
                    if (parent.IsDeleted)
                    {
                        return true;
                    }
                    return this.IsParentDeleted(parent);
                }
            }
            return false;
        }

        public bool IsParentDeleted(OrigamiContent page)
        {
            if (page.ParentId.HasValue)
            {
                var parent = Contents.ReadFromCache().Id(page.ParentId.Value);
                if (parent != null)
                {
                    if (parent.IsDeleted)
                    {
                        return true;
                    }
                    return this.IsParentDeleted(parent);
                }
            }
            return false;
        }

        public bool IsParentDeleted(OrigamiContentComment comment)
        {
            if (comment.ParentId.HasValue)
            {
                var parent = ContentComments.ReadFromCache().Id(comment.ParentId.Value);
                if (parent != null && parent.IsDeleted) return true;
                if (parent != null) return this.IsParentDeleted(parent);
            }
            return false;
        }



        /// <summary>
        /// Refreshes the caches and updates the counts for all repositories managed by the specified <see
        /// cref="ISuperRepository"/>.
        /// </summary>
        /// <remarks>This method performs the following operations: <list type="bullet">
        /// <item><description>Refreshes the cache for all repositories, including blogs, categories, pages, posts,
        /// videos, tags, users, and related entities.</description></item> <item><description>Updates the counts for
        /// page views, post views, video views, and comment views for both posts and videos.</description></item>
        /// </list> Use this method to ensure that all repository data is up-to-date and consistent, particularly after
        /// significant changes or updates.</remarks>
        /// <param name="_super">The <see cref="ISuperRepository"/> instance whose repositories will be refreshed.</param>
        public Result RefreshAllRepositories()
        {
            try
            {
                Blogs.RefreshCache();
                Categories.RefreshCache();
                ContentCategories.RefreshCache();
                ContentCommentReactions.RefreshCache();
                ContentComments.RefreshCache();
                ContentRatings.RefreshCache();
                Contents.RefreshCache();
                ContentTags.RefreshCache();
                QuickNotes.RefreshCache();
                Resumes.RefreshCache();
                Roles.RefreshCache();
                Settings.RefreshCache();
                SocialProfiles.RefreshCache();
                SpecialMessages.RefreshCache();
                SpecialPages.RefreshCache();
                Subscribers.RefreshCache();
                Tags.RefreshCache();
                Users.RefreshCache();

                if (this.AppFacade.Admin.GetValueOrDefault() == true)
                {
                    BackupAndRestores.RefreshCache();
                    UserPasswordResets.RefreshCache();
                    UserBlogs.RefreshCache();
                }

                var physicalViews = PhysicalPageViews.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();

                PhysicalPageViews.Update(physicalViews);

                return new();
            }
            catch (Exception ex)
            {
                return new Result(ex);
            }
        }

        /// <summary>
        /// Refreshes all search indexes for the repository.
        /// </summary>
        /// <remarks>This method iterates through all supported entities in the repository and recreates
        /// their respective search indexes. It is typically used to ensure that search functionality remains up-to-date
        /// after significant changes to the data.</remarks>
        /// <param name="_super">The repository instance that provides access to the various entities whose search indexes will be refreshed.</param>
        public Result RefreshAllSearchIndexes()
        {
            try
            {
                Blogs.CreateSearchIndex();
                Categories.CreateSearchIndex();
                ContentComments.CreateSearchIndex();
                Contents.CreateSearchIndex();
                Pages.CreateSearchIndex();
                QuickNotes.CreateSearchIndex();
                Roles.CreateSearchIndex();
                SocialProfiles.CreateSearchIndex();
                Tags.CreateSearchIndex();
                Users.CreateSearchIndex();
                return new();
            }
            catch (Exception ex)
            {
                return new Result(ex);
            }
        }

        public Result RegenerateNanoIds()
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                using (var transaction = new TransactionScope())
                {
                    // blogs
                    foreach (var blog in db.Set<OrigamiBlog>().AsNoTracking().ToList())
                    {
                        blog.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
                        var ctx = new DataOperationContext<OrigamiBlog>(OrigamiUser.AnonymousUser, blog);
                        Blogs.SmartSave(ctx, false);
                    }
                    // categories
                    foreach (var category in db.Set<OrigamiCategory>().AsNoTracking().ToList())
                    {
                        category.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
                        var ctx = new DataOperationContext<OrigamiCategory>(OrigamiUser.AnonymousUser, category);
                        Categories.SmartSave(ctx, false);
                    }
                    // pages
                    foreach (var page in db.Set<OrigamiPage>().AsNoTracking().ToList())
                    {
                        page.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
                        var ctx = new DataOperationContext<OrigamiPage>(OrigamiUser.AnonymousUser, page);
                        Pages.SmartSave(ctx, false);
                    }
                    // posts
                    foreach (var post in db.Set<OrigamiPost>().AsNoTracking().ToList())
                    {
                        post.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
                        var ctx = new DataOperationContext<OrigamiPost>(OrigamiUser.AnonymousUser, post);
                        Posts.SmartSave(ctx, false);
                    }
                    // videos
                    foreach (var video in db.Set<OrigamiVideo>().AsNoTracking().ToList())
                    {
                        video.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
                        var ctx = new DataOperationContext<OrigamiVideo>(OrigamiUser.AnonymousUser, video);
                        Videos.SmartSave(ctx, false);
                    }
                    // users
                    foreach (var user in db.Set<OrigamiUser>().AsNoTracking().ToList())
                    {
                        user.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
                        var ctx = new DataOperationContext<OrigamiUser>(OrigamiUser.AnonymousUser, user);
                        Users.SmartSave(ctx, false);
                    }
                    transaction.Complete();
                }

                return new();
            }
            catch (Exception ex)
            {
                return new Result(ex);
            }

        }

        protected IEnumerable<IId> GetIds()
        {
            return [
                .. Blogs.ReadFromCache(),
                .. Categories.ReadFromCache(),
                .. Pages.ReadFromCache(),
                .. Posts.ReadFromCache(),
                .. Videos.ReadFromCache(),
                .. Users.ReadFromCache() ];
        }

        protected IEnumerable<INanoId> GetNanoIds()
        {
            return [
                .. Blogs.ReadFromCache(),
                .. Categories.ReadFromCache(),
                .. Pages.ReadFromCache(),
                .. Posts.ReadFromCache(),
                .. Videos.ReadFromCache(),
                .. Users.ReadFromCache() ];
        }
    }
}
