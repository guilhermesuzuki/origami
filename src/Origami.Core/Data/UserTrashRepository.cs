using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserTrashRepository :
        RepositoryOuterLayer<OrigamiUserTrash>,
        IUserTrashRepository
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IPageViewRepository _pageViewRepository;
        private readonly IPhysicalPageRepository _physicalPageRepository;
        private readonly IPhysicalPageViewRepository _physicalPageViewRepository;
        private readonly IPostCategoryRepository _postCategoryRepository;
        private readonly IPostCommentReactionRepository _postCommentReactionRepository;
        private readonly IPostCommentRepository _postCommentRepository;
        private readonly IPostRatingRepository _postRatingRepository;
        private readonly IPostRepository _postRepository;
        private readonly IPostTagRepository _postTagRepository;
        private readonly IPostViewRepository _postViewRepository;
        private readonly IQuickNoteRepository _quickNoteRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISettingsRepository _blogSettingsRepository;
        private readonly ISocialProfileRepository _socialProfileRepository;
        private readonly ISpecialMessageRepository _specialMessageRepository;
        private readonly ISpecialPageRepository _specialPageRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly IUserContentRepository _userContentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserViewRepository _userViewRepository;
        private readonly IVideoCategoryRepository _videoCategoryRepository;
        private readonly IVideoCommentReactionRepository _videoCommentReactionRepository;
        private readonly IVideoCommentRepository _videoCommentRepository;
        private readonly IVideoRatingRepository _videoRatingRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IVideoTagRepository _videoTagRepository;
        private readonly IVideoViewRepository _videoViewRepository;
        private readonly IWhatToSeeNextRepository _whatToSeeNextRepository;

        public UserTrashRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IBlogRepository blogRepository,
            ICategoryRepository categoryRepository,
            IDirectoryRepository directoryRepository,
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
            IRoleRepository roleRepository,
            ISettingsRepository blogSettingsRepository,
            ISocialProfileRepository socialProfileRepository,
            ISpecialMessageRepository specialMessageRepository,
            ISpecialPageRepository specialPageRepository,
            ISubscriberRepository subscriberRepository,
            ITagRepository tagRepository,
            IUserActivityRepository userActivityRepository,
            IUserContentRepository userContentRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IUserViewRepository userViewRepository,
            IVideoCategoryRepository videoCategoryRepository,
            IVideoCommentReactionRepository videoCommentReactionRepository,
            IVideoCommentRepository videoCommentRepository,
            IVideoRatingRepository videoRatingRepository,
            IVideoRepository videoRepository,
            IVideoTagRepository videoTagRepository,
            IVideoViewRepository videoViewRepository,
            IWhatToSeeNextRepository whatToSeeNextRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _blogRepository = blogRepository;
            _blogSettingsRepository = blogSettingsRepository;
            _categoryRepository = categoryRepository;
            _directoryRepository = directoryRepository;
            _fileRepository = fileRepository;
            _pageRepository = pageRepository;
            _pageViewRepository = pageViewRepository;
            _physicalPageRepository = physicalPageRepository;
            _physicalPageViewRepository = physicalPageViewRepository;
            _postCategoryRepository = postCategoryRepository;
            _postCommentReactionRepository = postCommentReactionRepository;
            _postCommentRepository = postCommentRepository;
            _postRatingRepository = postRatingRepository;
            _postRepository = postRepository;
            _postTagRepository = postTagRepository;
            _postViewRepository = postViewRepository;
            _quickNoteRepository = quickNoteRepository;
            _roleRepository = roleRepository;
            _socialProfileRepository = socialProfileRepository;
            _specialMessageRepository = specialMessageRepository;
            _specialPageRepository = specialPageRepository;
            _subscriberRepository = subscriberRepository;
            _tagRepository = tagRepository;
            _userActivityRepository = userActivityRepository;
            _userContentRepository = userContentRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _userViewRepository = userViewRepository;
            _videoCategoryRepository = videoCategoryRepository;
            _videoCommentReactionRepository = videoCommentReactionRepository;
            _videoCommentRepository = videoCommentRepository;
            _videoRatingRepository = videoRatingRepository;
            _videoRepository = videoRepository;
            _videoTagRepository = videoTagRepository;
            _videoViewRepository = videoViewRepository;
            _whatToSeeNextRepository = whatToSeeNextRepository;
        }

        public override string ReadPermission => nameof(OrigamiRole.ViewTrashes);

        public override List<OrigamiUserTrash> Search(string searchTerm)
        {
            using var db = DbContextFactory.CreateDbContext();

            var query = from x in db.Set<OrigamiUserTrash>().AsNoTracking()
                        where x.Type.Contains(searchTerm) ||
                              x.Name.Contains(searchTerm) ||
                              x.Title.Contains(searchTerm) ||
                              x.Content.Contains(searchTerm)
                        orderby x.Type, string.IsNullOrWhiteSpace(x.Title) == false ? x.Title : x.Name
                        select x;

            return query.ToList();
        }

        public override Result<OrigamiUserTrash> SmartPurge(DataOperationContext<OrigamiUserTrash> ctx, bool checkPermission)
        {
            if (ctx.Entity.Type.Like("Blog") == true)
            {
                return _purge(_blogRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Page") == true)
            {
                return _purge(_pageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Post") == true)
            {
                return _purge(_postRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Video") == true)
            {
                return _purge(_videoRepository, ctx);
            }

            if (ctx.Entity.Type.Like("PostComment") == true)
            {
                return _purge(_postCommentRepository, ctx);
            }

            if (ctx.Entity.Type.Like("VideoComment") == true)
            {
                return _purge(_videoCommentRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Category") == true)
            {
                return _purge(_categoryRepository, ctx);
            }

            if (ctx.Entity.Type.Like("User") == true)
            {
                return _purge(_userRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Role") == true)
            {
                return _purge(_roleRepository, ctx);
            }

            if (ctx.Entity.Type.Like("QuickNote") == true)
            {
                return _purge(_quickNoteRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialPage") == true)
            {
                return _purge(_specialPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialMessage") == true)
            {
                return _purge(_specialMessageRepository, ctx);
            }

            throw new NotImplementedException();
        }

        public override Result<OrigamiUserTrash> SmartRestore(DataOperationContext<OrigamiUserTrash> ctx, bool checkPermission)
        {
            if (ctx.Entity.Type.Like("Blog") == true)
            {
                return _restore(_blogRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Page") == true)
            {
                return _restore(_pageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Post") == true)
            {
                return _restore(_postRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Video") == true)
            {
                return _restore(_videoRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Post") == true)
            {
                return _restore(_postCommentRepository, ctx);
            }

            if (ctx.Entity.Type.Like("VideoComment") == true)
            {
                return _restore(_videoCommentRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Category") == true)
            {
                return _restore(_categoryRepository, ctx);
            }

            if (ctx.Entity.Type.Like("User") == true)
            {
                return _restore(_userRepository, ctx);
            }

            if (ctx.Entity.Type.Like("Role") == true)
            {
                return _restore(_roleRepository, ctx);
            }

            if (ctx.Entity.Type.Like("QuickNote") == true)
            {
                return _restore(_quickNoteRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialPage") == true)
            {
                return _restore(_specialPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialMessage") == true)
            {
                return _restore(_specialMessageRepository, ctx);
            }

            throw new NotImplementedException();
        }

        private Result<OrigamiUserTrash> _purge<T>(IRepository<T> repo, DataOperationContext<OrigamiUserTrash> trash)
            where T : class, IId, new()
        {
            var hub = new Result<OrigamiUserTrash>(trash.Entity);
            var entity = repo.ReadFromDatabase(trash.Entity);
            var ctx = new DataOperationContext<T>(trash.User, trash.DateTime, entity ?? new());
            return repo.SmartPurge(ctx, true).Push(hub);
        }

        private Result<OrigamiUserTrash> _restore<T>(IRepository<T> repo, DataOperationContext<OrigamiUserTrash> trash)
            where T : class, IId, new()
        {
            var hub = new Result<OrigamiUserTrash>(trash.Entity);
            var entity = repo.ReadFromDatabase(trash.Entity);
            var ctx = new DataOperationContext<T>(trash.User, trash.DateTime, entity ?? new());
            if (entity != null)
            {
                return repo.SmartRestore(ctx, true).Push(hub);
            }
            return new(trash.Entity) { Error = Text.Original("Unable to restore trash") };
        }
    }
}
