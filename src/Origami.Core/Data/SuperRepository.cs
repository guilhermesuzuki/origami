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
            IPageViewRepository pageViewRepository,
            IPhysicalPageRepository physicalPageRepository,
            IPhysicalPageViewRepository physicalPageViewRepository,
            IPostCategoryRepository postCategoryRepository,
            IPostCommentReactionRepository postCommentReactionRepository,
            IPostCommentRepository postCommentRepository,
            IPostRatingRepository postRatingRepository,
            IPostRepository postRepository,
            IPostTagRepository postTagRepository,
            IPostViewRepository postViewRepository,
            IQuickNoteRepository quickNoteRepository,
            IResumeRepository resumeRepository,
            IRightRepository rightRepository,
            IRoleRepository roleRepository,
            ISettingsRepository settingsRepository,
            ISocialProfileRepository socialProfileRepository,
            ISpecialMessageRepository specialMessageRepository,
            ISpecialPageRepository specialPageRepository,
            ISpecialPageViewRepository specialPageViewRepository,
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
            IVideoCategoryRepository videoCategoryRepository,
            IVideoCommentReactionRepository videoCommentReactionRepository,
            IVideoCommentRepository videoCommentRepository,
            IVideoRatingRepository videoRatingRepository,
            IVideoRepository videoRepository,
            IVideoTagRepository videoTagRepository,
            IVideoViewRepository videoViewRepository,
            IWhatToSeeNextRepository whatToSeeNextRepository,
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
            PageViews = pageViewRepository;
            PhysicalPages = physicalPageRepository;
            PhysicalPageViews = physicalPageViewRepository;
            PostCategories = postCategoryRepository;
            PostCommentReactions = postCommentReactionRepository;
            PostComments = postCommentRepository;
            PostRatings = postRatingRepository;
            Posts = postRepository;
            PostTags = postTagRepository;
            PostViews = postViewRepository;
            QuickNotes = quickNoteRepository;
            Resumes = resumeRepository;
            Rights = rightRepository;
            Roles = roleRepository;
            Settings = settingsRepository;
            SocialProfiles = socialProfileRepository;
            SpecialMessages = specialMessageRepository;
            SpecialPages = specialPageRepository;
            SpecialPageViews = specialPageViewRepository;
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
            VideoCategories = videoCategoryRepository;
            VideoCommentReactions = videoCommentReactionRepository;
            VideoComments = videoCommentRepository;
            VideoRatings = videoRatingRepository;
            Videos = videoRepository;
            VideoTags = videoTagRepository;
            VideoViews = videoViewRepository;
            WhatToSeeNext = whatToSeeNextRepository;
        }
        public IAppFacade AppFacade { get; }
        public IBackupRestoreRepository BackupAndRestores { get; }
        public IBlogRepository Blogs { get; }
        public ICategoryRepository Categories { get; }
        public IConfiguration Configurations { get; }
        public IDbContextFactory<OrigamiDbContext> DbContextFactory { get; }
        public IDirectoryRepository Directories { get; }
        public IEmailRepository Emails { get; }
        public IFileRepository Files { get; }
        public bool MaintenanceLockout => this.GetMaintenancePages().Any();
        public IPageRepository Pages { get; }
        public IPageViewRepository PageViews { get; }
        public IPhysicalPageRepository PhysicalPages { get; }
        public IPhysicalPageViewRepository PhysicalPageViews { get; }
        public IPostCategoryRepository PostCategories { get; }
        public IPostCommentReactionRepository PostCommentReactions { get; }
        public IPostCommentRepository PostComments { get; }
        public IPostRatingRepository PostRatings { get; }
        public IPostRepository Posts { get; }
        public IPostTagRepository PostTags { get; }
        public IPostViewRepository PostViews { get; }
        public IQuickNoteRepository QuickNotes { get; }
        public IResumeRepository Resumes { get; }
        public IRightRepository Rights { get; }
        public IRoleRepository Roles { get; }
        public ISettingsRepository Settings { get; }
        public ISocialProfileRepository SocialProfiles { get; }
        public ISpecialMessageRepository SpecialMessages { get; }
        public ISpecialPageRepository SpecialPages { get; }
        public ISpecialPageViewRepository SpecialPageViews { get; }
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
        public IVideoCategoryRepository VideoCategories { get; }
        public IVideoCommentReactionRepository VideoCommentReactions { get; }
        public IVideoCommentRepository VideoComments { get; }
        public IVideoRatingRepository VideoRatings { get; }
        public IVideoRepository Videos { get; }
        public IVideoTagRepository VideoTags { get; }
        public IVideoViewRepository VideoViews { get; }
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

        public IEnumerable<OrigamiCategory> GetCategories(OrigamiPost post)
        {
            return from a in PostCategories.ReadFromCache()
                   join b in Categories.ReadFromCache() on a.CategoryId equals b.Id
                   where a.PostId == post.Id
                   where b.IsDeleted == false
                   where this.IsParentDeleted(b) == false
                   orderby b.Name
                   select b;
        }

        public IEnumerable<OrigamiCategory> GetCategories(OrigamiVideo video)
        {
            return from a in VideoCategories.ReadFromCache()
                   join b in Categories.ReadFromCache() on a.CategoryId equals b.Id
                   where a.VideoId == video.Id
                   where b.IsDeleted == false
                   where this.IsParentDeleted(b) == false
                   orderby b.Name
                   select b;
        }

        public IEnumerable<BaseComment> GetComments(Guid blog)
        {
            var result = new List<BaseComment>();

            result.AddRange(from x in PostComments.ReadFromCache()
                            join y in Posts.ReadFromCache() on x.PostId equals y.Id
                            where y.BlogId == blog
                            select x);

            result.AddRange(from x in VideoComments.ReadFromCache()
                            join y in Videos.ReadFromCache() on x.VideoId equals y.Id
                            where y.BlogId == blog
                            select x);

            return result;
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
            return from a in PostTags.ReadFromCache()
                   join b in Posts.ReadFromCache() on a.PostId equals b.Id
                   where a.Tag.Like(tag.Name)
                   where b.BlogId == tag.BlogId
                   select b;
        }

        public IEnumerable<OrigamiPost> GetPosts(OrigamiCategory category)
        {
            return from a in PostCategories.ReadFromCache()
                   join b in Posts.ReadFromCache() on a.PostId equals b.Id
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

        public IEnumerable<BaseComment> GetReplies(BaseComment comment)
        {
            if (comment is OrigamiPostComment)
            {
                var replies = from x in PostComments.ReadFromCache().NonDeleted()
                              where x.ParentId == comment.Id
                              where x.IsSpam == false
                              where x.IsDeleted == false
                              where x.IsApproved == true
                              orderby x.DateCreated descending
                              select x;

                return replies;
            }
            if (comment is OrigamiVideoComment)
            {
                var replies = from x in VideoComments.ReadFromCache().NonDeleted()
                              where x.ParentId == comment.Id
                              where x.IsSpam == false
                              where x.IsDeleted == false
                              where x.IsApproved == true
                              orderby x.DateCreated descending
                              select x;

                return replies;
            }
            return [];
        }

        public OrigamiSubscriber? GetSubscriber(OrigamiSocialProfile socialProfile)
        {
            return Subscribers.ReadFromCache().NonDeleted()
                .Where(x => x.IsVerified == true)
                .Where(x => x.SocialProfileId == socialProfile.Id)
                .FirstOrDefault();
        }

        public IEnumerable<OrigamiPostTag> GetTags(OrigamiPost post)
        {
            return from x in PostTags.ReadFromCache()
                   where x.PostId == post.Id
                   orderby x.Tag
                   select x;
        }

        public IEnumerable<OrigamiVideoTag> GetTags(OrigamiVideo video)
        {
            return from x in VideoTags.ReadFromCache()
                   where x.VideoId == video.Id
                   orderby x.Tag
                   select x;
        }

        public IEnumerable<OrigamiVideo> GetVideos(OrigamiTag tag)
        {
            return from a in VideoTags.ReadFromCache()
                   join b in Videos.ReadFromCache() on a.VideoId equals b.Id
                   where a.Tag.Like(tag.Name)
                   where b.BlogId == tag.BlogId
                   select b;
        }

        public IEnumerable<OrigamiVideo> GetVideos(OrigamiCategory category)
        {
            var query = VideoCategories.ReadFromCache().Where(x => x.CategoryId == category.Id).Select(x => x.VideoId).ToList();
            return from v in Videos.ReadFromCache() join id in query on v.Id equals id select v;
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

        public bool IsParentDeleted(OrigamiPage page)
        {
            if (page.ParentId.HasValue)
            {
                var parent = Pages.ReadFromCache().Id(page.ParentId.Value);
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

        public bool IsParentDeleted(BaseComment comment)
        {
            if (comment is OrigamiPostComment pcomment)
            {
                if (pcomment.ParentId.HasValue)
                {
                    var parent = PostComments.ReadFromCache().Id(pcomment.ParentId.Value);
                    if (parent != null && parent.IsDeleted) return true;
                    if (parent != null) return this.IsParentDeleted(parent);
                }
            }
            if (comment is OrigamiVideoComment vcomment)
            {
                if (vcomment.ParentId.HasValue)
                {
                    var parent = VideoComments.ReadFromCache().Id(vcomment.ParentId.Value);
                    if (parent != null && parent.IsDeleted) return true;
                    if (parent != null) return this.IsParentDeleted(parent);
                }
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
                Pages.RefreshCache();
                PostCategories.RefreshCache();
                PostCommentReactions.RefreshCache();
                PostComments.RefreshCache();
                PostRatings.RefreshCache();
                Posts.RefreshCache();
                PostTags.RefreshCache();
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
                VideoCategories.RefreshCache();
                VideoCommentReactions.RefreshCache();
                VideoComments.RefreshCache();
                VideoRatings.RefreshCache();
                Videos.RefreshCache();
                VideoTags.RefreshCache();

                if (this.AppFacade.Admin.GetValueOrDefault() == true)
                {
                    BackupAndRestores.RefreshCache();
                    UserPasswordResets.RefreshCache();
                    UserBlogs.RefreshCache();
                }

                var pageViews = PageViews.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();
                var postViews = PostViews.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();
                var videoViews = VideoViews.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();
                var postCommentViews = PostComments.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();
                var videoCommentViews = VideoComments.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();
                var physicalViews = PhysicalPageViews.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();
                var specialPageViews = SpecialPageViews.FastRead().ConfigureAwait(false).GetAwaiter().GetResult();

                PageViews.Update(pageViews);
                PostViews.Update(postViews);
                VideoViews.Update(videoViews);
                PostComments.Update(postCommentViews);
                VideoComments.Update(videoCommentViews);
                PhysicalPageViews.Update(physicalViews);
                SpecialPageViews.Update(specialPageViews);

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
                Pages.CreateSearchIndex();
                PostComments.CreateSearchIndex();
                Posts.CreateSearchIndex();
                PostTags.CreateSearchIndex();
                QuickNotes.CreateSearchIndex();
                Roles.CreateSearchIndex();
                SocialProfiles.CreateSearchIndex();
                Tags.CreateSearchIndex();
                Users.CreateSearchIndex();
                VideoComments.CreateSearchIndex();
                Videos.CreateSearchIndex();
                VideoTags.CreateSearchIndex();
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
