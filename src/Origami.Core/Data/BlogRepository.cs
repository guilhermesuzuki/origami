using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Origami.Core.Models;
using System.Linq.Dynamic.Core;

namespace Origami.Core.Data
{
    public class BlogRepository :
        RepositoryOuterLayer<OrigamiBlog>,
        IBlogRepository
    {
        protected readonly IValidator<OrigamiBlog> _validator;
        protected readonly IBlogRollRepository _blogRollRepository;
        protected readonly ICategoryRepository _categoryRepository;
        protected readonly IConfiguration _configuration;
        protected readonly IPageRepository _pageRepository;
        protected readonly IPingServiceRepository _pingServiceRepository;
        protected readonly IPostRepository _postRepository;
        protected readonly IRoleRepository _roleRepository;
        protected readonly IUserRepository _userRepository;
        protected readonly IVideoRepository _videoRepository;

        public BlogRepository(
            IValidator<OrigamiBlog> validator,
            IBlogRollRepository blogRollRepository,
            ICategoryRepository categoryRepository,
            IConfiguration configuration,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IPageRepository pageRepository,
            IPingServiceRepository pingServiceRepository,
            IPostRepository postRepository,
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IVideoRepository videoRepository,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _configuration = configuration;
            _blogRollRepository = blogRollRepository;
            _categoryRepository = categoryRepository;
            _pageRepository = pageRepository;
            _pingServiceRepository = pingServiceRepository;
            _postRepository = postRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _videoRepository = videoRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewBlogs);
        public override string DeletePermission => nameof(OrigamiRole.DeleteBlogs);
        public override string PurgePermission => nameof(OrigamiRole.PurgeBlogs);
        public override string ReadPermission => nameof(OrigamiRole.ViewBlogs);
        public override string RestorePermission => nameof(OrigamiRole.RestoreBlogs);
        public override string UpdatePermission => nameof(OrigamiRole.EditBlogs);

        public Result<OrigamiBlog> Activate(DataOperationContext<OrigamiBlog> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.ActivateBlogs));
                if (permission.Ok == false) return permission;
            }

            var fresh = ReadFromDatabase().Id(ctx.Entity.Id);
            if (fresh != null)
            {
                if (fresh.IsActive == false)
                {
                    ctx.Entity.IsActive = true;
                    return SmartUpdate(ctx, false);
                }
                return new(ctx.Entity, Text.Original("Blog is already activated"));
            }

            return new(ctx.Entity, Text.Original("Blog could not be found"));
        }

        public override Result<OrigamiBlog> CreateValidation(DataOperationContext<OrigamiBlog> ctx)
        {
            var validation = new Result<OrigamiBlog>(ctx.Entity, _validator);
            this.ValidateSlug(ctx).Push(validation);
            return validation;
        }

        public Result<OrigamiBlog> Deactivate(DataOperationContext<OrigamiBlog> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.DeactivateBlogs));
                if (permission.Ok == false) return permission;
            }

            var fresh = ReadFromDatabase().Id(ctx.Entity.Id);
            if (fresh != null)
            {
                if (fresh is { IsPrimary: true })
                {
                    return new(ctx.Entity, Text.Original("Primary blog cannot be deactivated"));
                }

                if (fresh is { IsActive: true })
                {
                    ctx.Entity.IsActive = false;
                    return SmartUpdate(ctx, false);
                }
                return new(ctx.Entity, Text.Original("Blog is already deactivated"));
            }

            return new(ctx.Entity, Text.Original("Blog could not be found"));
        }

        public override Result<OrigamiBlog> DeleteValidation(DataOperationContext<OrigamiBlog> ctx)
        {
            var hub = base.DeleteValidation(ctx);
            if (hub.Ok)
            {
                var fresh = ReadFromDatabase().Id(ctx.Entity.Id);
                if (fresh == null)
                {
                    hub.ErrorMessage = Text.Original("Blog could not be found");
                }
                else if (fresh is { IsPrimary: true })
                {
                    hub.ErrorMessage = Text.Original("Primary blog cannot be deleted");
                }
            }
            return hub;
        }

        public string DirectoryForScalingImages()
        {
            return $"/files/{typeof(OrigamiBlog).GetPlural().ToLower()}/scaling/";
        }

        public OrigamiBlog GetPrimary()
        {
            using var db = DbContextFactory.CreateDbContext();
            return db.Blogs.Single(x => x.IsPrimary);
        }

        public Result<OrigamiBlog> SetPrimary(DataOperationContext<OrigamiBlog> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.MarkBlogAsPrimary));
                if (permission.Ok == false) return permission;
            }

            using var db = DbContextFactory.CreateDbContext();

            db.Blogs.Where(x => x.IsPrimary).ExecuteUpdate(setters => setters.SetProperty(x => x.IsPrimary, false));
            db.Blogs.Where(x => x.Id == ctx.Entity.Id).ExecuteUpdate(setters => setters.SetProperty(x => x.IsPrimary, true));

            //sets the IsPrimary property to true for the current blog
            ctx.Entity.IsPrimary = true;

            //refreshes the cache
            RefreshCache();

            //returns the updated blog entity
            return new(ctx.Entity);
        }

        public Result SortThem(DataOperationContext<IEnumerable<Guid>> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.EditBlogs));
                if (permission.Ok == false) return permission;
            }

            var blogs = from i in ctx.Entity
                        join b in ReadFromDatabase().NonDeleted().Active() on i equals b.Id
                        select new { b, i };

            if (ctx.Entity.Count() != blogs.Count())
            {
                return new() { ErrorMessage = Text.Original("Invalid ids (may contain deleted, inactive blog ids)") };
            }

            try
            {
                var hub = new Result();
                using (var db = DbContextFactory.CreateDbContext())
                {
                    var index = 0;
                    foreach (var blog in blogs)
                    {
                        if (hub.Ok == false) return hub;
                        var before = ReadFromDatabase().Id(blog.b.Id)!;
                        var update = ReadFromDatabase().Id(blog.b.Id)!;
                        update.Order = ++index;
                        SmartUpdate(new DataOperationContext<OrigamiBlog>(ctx.User, DateTime.UtcNow, update, before), false).Push(hub);
                    }
                }
                return hub;
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public Result SortThemWithDefault(DataOperationContext ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.EditBlogs));
                if (permission.Ok == false) return permission;
            }

            var blogs = ReadFromDatabase().NonDeleted().Active();

            try
            {
                var hub = new Result();
                using (var db = DbContextFactory.CreateDbContext())
                {
                    foreach (var blog in blogs)
                    {
                        if (hub.Ok == false) return hub;
                        var before = ReadFromDatabase().Id(blog.Id)!;
                        var update = ReadFromDatabase().Id(blog.Id)!;
                        update.Order = null;
                        SmartUpdate(new DataOperationContext<OrigamiBlog>(ctx.User, DateTime.UtcNow, update, before), false).Push(hub);
                    }
                }
                return hub;
            }
            catch (Exception ex)
            {
                return new(ex);
            }
        }

        public override Result<OrigamiBlog> UpdateValidation(DataOperationContext<OrigamiBlog> ctx)
        {
            var validation = new Result<OrigamiBlog>(ctx.Entity, _validator);
            this.ValidateSlug(ctx).Push(validation);
            return validation;
        }

        public override void PurgeRelationshipsFromCache(OrigamiBlog entity)
        {
            var categories = _categoryRepository.ReadFromCache().Blog(entity.Id).ToList();
            var pages = _pageRepository.ReadFromCache().Blog(entity.Id).ToList();
            var pingServices = _pingServiceRepository.ReadFromCache().Blog(entity.Id).ToList();
            var posts = _postRepository.ReadFromCache().Blog(entity.Id).ToList();
            var videos = _videoRepository.ReadFromCache().Blog(entity.Id).ToList();

            categories.Each(_categoryRepository.PurgeCache);
            pages.Each(_pageRepository.PurgeCache);
            pingServices.Each(_pingServiceRepository.PurgeCache);
            posts.Each(_postRepository.PurgeCache);
            videos.Each(_videoRepository.PurgeCache);
        }

        public override Result<OrigamiBlog> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiBlog> ctx)
        {
            var hub = base.PurgeRelationshipsFromDatabase(ctx);

            var categories = _categoryRepository.ReadFromDatabase().Blog(ctx.Entity.Id).ToList();
            var pages = _pageRepository.ReadFromDatabase().Blog(ctx.Entity.Id).ToList();
            var pingServices = _pingServiceRepository.ReadFromDatabase().Blog(ctx.Entity.Id).ToList();
            var posts = _postRepository.ReadFromDatabase().Blog(ctx.Entity.Id).ToList();
            var videos = _videoRepository.ReadFromDatabase().Blog(ctx.Entity.Id).ToList();

            categories.GetContexts(ctx).Call(_categoryRepository.SmartPurge, false).Push(hub);
            pages.GetContexts(ctx).Call(_pageRepository.SmartPurge, false).Push(hub);
            pingServices.GetContexts(ctx).Call(_pingServiceRepository.SmartPurge, false).Push(hub);
            posts.GetContexts(ctx).Call(_postRepository.SmartPurge, false).Push(hub);
            videos.GetContexts(ctx).Call(_videoRepository.SmartPurge, false).Push(hub);

            using (var db = DbContextFactory.CreateDbContext())
            {
                hub.RowsAffected += db.CustomFields.Where(x => x.BlogId == ctx.Entity.Id).ExecuteDelete();
                hub.RowsAffected += db.DataStoreSettings.Where(x => x.BlogId == ctx.Entity.Id).ExecuteDelete();
                hub.RowsAffected += db.QuickNotes.Where(x => x.BlogId == ctx.Entity.Id).ExecuteDelete();
                hub.RowsAffected += db.QuickSettings.Where(x => x.BlogId == ctx.Entity.Id).ExecuteDelete();
                hub.RowsAffected += db.StopWords.Where(x => x.BlogId == ctx.Entity.Id).ExecuteDelete();
            }

            return hub;
        }
    }
}
