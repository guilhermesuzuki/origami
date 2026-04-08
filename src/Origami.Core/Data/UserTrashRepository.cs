using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserTrashRepository :
        RepositoryOuterLayer<OrigamiUserTrash>,
        IUserTrashRepository
    {
        protected readonly IHubContentRepository<HubContentPage> _hubContentPageRepository;
        protected readonly IHubContentRepository<HubContentPost> _hubContentPostRepository;
        protected readonly IHubContentRepository<HubContentSpecialMessage> _hubContentSpecialMessageRepository;
        protected readonly IHubContentRepository<HubContentSpecialPage> _hubContentSpecialPageRepository;
        protected readonly IHubContentRepository<HubContentVideo> _hubContentVideoRepository;
        protected readonly IContentCommentRepository _contentCommentRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ISettingsRepository _blogSettingsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IPhysicalPageRepository _physicalPageRepository;
        private readonly IPhysicalPageViewRepository _physicalPageViewRepository;
        private readonly IPostRepository _postRepository;
        private readonly IQuickNoteRepository _quickNoteRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISocialProfileRepository _socialProfileRepository;
        private readonly ISpecialMessageRepository _specialMessageRepository;
        private readonly ISpecialPageRepository _specialPageRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserViewRepository _userViewRepository;
        private readonly IVideoRepository _videoRepository;
        private readonly IWhatToSeeNextRepository _whatToSeeNextRepository;

        public UserTrashRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IBlogRepository blogRepository,
            ICategoryRepository categoryRepository,
            IContentRepository contentRepository,
            IDirectoryRepository directoryRepository,
            IFileRepository fileRepository,
            IPageRepository pageRepository,
            IPhysicalPageRepository physicalPageRepository,
            IPhysicalPageViewRepository physicalPageViewRepository,
            IPostRepository postRepository,
            IQuickNoteRepository quickNoteRepository,
            IRoleRepository roleRepository,
            ISettingsRepository blogSettingsRepository,
            ISocialProfileRepository socialProfileRepository,
            ISpecialMessageRepository specialMessageRepository,
            ISpecialPageRepository specialPageRepository,
            ISubscriberRepository subscriberRepository,
            IUserActivityRepository userActivityRepository,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IUserViewRepository userViewRepository,
            IVideoRepository videoRepository,
            IWhatToSeeNextRepository whatToSeeNextRepository,

            IHubContentRepository<HubContentPage> hubContentPageRepository,
            IHubContentRepository<HubContentPost> hubContentPostRepository,
            IHubContentRepository<HubContentSpecialMessage> hubContentSpecialMessageRepository,
            IHubContentRepository<HubContentSpecialPage> hubContentSpecialPageRepository,
            IHubContentRepository<HubContentVideo> hubContentVideoRepository,
            IContentCommentRepository contentCommentRepository,

            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _blogRepository = blogRepository;
            _blogSettingsRepository = blogSettingsRepository;
            _categoryRepository = categoryRepository;
            _contentRepository = contentRepository;
            _directoryRepository = directoryRepository;
            _fileRepository = fileRepository;
            _pageRepository = pageRepository;
            _physicalPageRepository = physicalPageRepository;
            _physicalPageViewRepository = physicalPageViewRepository;
            _postRepository = postRepository;
            _quickNoteRepository = quickNoteRepository;
            _roleRepository = roleRepository;
            _socialProfileRepository = socialProfileRepository;
            _specialMessageRepository = specialMessageRepository;
            _specialPageRepository = specialPageRepository;
            _subscriberRepository = subscriberRepository;
            _userActivityRepository = userActivityRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _userViewRepository = userViewRepository;
            _videoRepository = videoRepository;
            _whatToSeeNextRepository = whatToSeeNextRepository;

            _hubContentPageRepository = hubContentPageRepository;
            _hubContentPostRepository = hubContentPostRepository;
            _hubContentSpecialMessageRepository = hubContentSpecialMessageRepository;
            _hubContentSpecialPageRepository = hubContentSpecialPageRepository;
            _hubContentVideoRepository = hubContentVideoRepository;
            _contentCommentRepository = contentCommentRepository;
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

            if (ctx.Entity.Type.Like("OrigamiPage") == true)
            {
                return _purge(_hubContentPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiPost") == true)
            {
                return _purge(_hubContentPostRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiSpecialMessage") == true)
            {
                return _purge(_hubContentSpecialMessageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiSpecialPage") == true)
            {
                return _purge(_hubContentSpecialPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiVideo") == true)
            {
                return _purge(_hubContentVideoRepository, ctx);
            }

            if (ctx.Entity.Type.Like("ContentComment") == true)
            {
                return _purge(_contentCommentRepository, ctx);
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

            throw new NotImplementedException();
        }

        public override Result<OrigamiUserTrash> SmartRestore(DataOperationContext<OrigamiUserTrash> ctx, bool checkPermission)
        {
            if (ctx.Entity.Type.Like("Blog") == true)
            {
                return _restore(_blogRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiPage") == true)
            {
                return _restore(_hubContentPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiPost") == true)
            {
                return _restore(_hubContentPostRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiSpecialMessage") == true)
            {
                return _restore(_hubContentSpecialMessageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiSpecialPage") == true)
            {
                return _restore(_hubContentSpecialPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("OrigamiVideo") == true)
            {
                return _restore(_hubContentVideoRepository, ctx);
            }

            if (ctx.Entity.Type.Like("ContentComment") == true)
            {
                return _restore(_contentCommentRepository, ctx);
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

            throw new NotImplementedException();
        }

        private Result<OrigamiUserTrash> _purge<T>(IRepository<T> repo, DataOperationContext<OrigamiUserTrash> trash)
            where T : class, IId
        {
            var hub = new Result<OrigamiUserTrash>(trash.Entity);
            var entity = repo.ReadFromDatabase(trash.Entity);
            var ctx = new DataOperationContext<T>(trash.User, trash.DateTime, entity ?? Activator.CreateInstance<T>());
            return repo.SmartPurge(ctx, true).Push(hub);
        }

        private Result<OrigamiUserTrash> _purge<T>(IHubContentRepository<T> repo, DataOperationContext<OrigamiUserTrash> trash)
            where T : class, IId
        {
            var hub = new Result<OrigamiUserTrash>(trash.Entity);
            var entity = repo.Get(trash.Entity);
            if (entity != null) repo.Purge(entity, trash.User).Push(hub);
            return hub;
        }

        private Result<OrigamiUserTrash> _restore<T>(IRepository<T> repo, DataOperationContext<OrigamiUserTrash> trash)
            where T : class, IId
        {
            var hub = new Result<OrigamiUserTrash>(trash.Entity);
            var entity = repo.ReadFromDatabase(trash.Entity);
            var ctx = new DataOperationContext<T>(trash.User, trash.DateTime, entity ?? Activator.CreateInstance<T>());
            if (entity != null)
            {
                return repo.SmartRestore(ctx, true).Push(hub);
            }
            return new(trash.Entity) { Error = Text.Original("Unable to restore trash") };
        }

        private Result<OrigamiUserTrash> _restore<T>(IHubContentRepository<T> repo, DataOperationContext<OrigamiUserTrash> trash)
            where T : class, IId
        {
            var hub = new Result<OrigamiUserTrash>(trash.Entity);
            var entity = repo.Get(trash.Entity);
            if (entity != null) repo.Restore(entity, trash.User).Push(hub);
            return hub;
        }
    }
}
