using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;
using System.Linq.Dynamic.Core;

namespace Origami.Core.Data
{
    public class BlogRepository :
        RepositoryOuterLayer<OrigamiBlog>,
        IBlogRepository
    {
        protected readonly ICategoryRepository _categoryRepository;
        protected readonly IHubContentRepository<HubContentPage> _hubPageRepository;
        protected readonly IHubContentRepository<HubContentPost> _hubPostRepository;
        protected readonly IHubContentRepository<HubContentQuickNote> _hubQuickNoteRepository;
        protected readonly IHubContentRepository<HubContentVideo> _hubVideoRepository;
        protected readonly IValidator<OrigamiBlog> _validator;

        public BlogRepository(
            IValidator<OrigamiBlog> validator,
            ICategoryRepository categoryRepository,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IHubContentRepository<HubContentPage> hubPageRepository,
            IHubContentRepository<HubContentPost> hubPostRepository,
            IHubContentRepository<HubContentQuickNote> hubQuickNoteRepository,
            IHubContentRepository<HubContentVideo> hubVideoRepository,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _categoryRepository = categoryRepository;
            _hubPageRepository = hubPageRepository;
            _hubPostRepository = hubPostRepository;
            _hubVideoRepository = hubVideoRepository;
            _hubQuickNoteRepository = hubQuickNoteRepository;
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

            var fresh = ReadFromDatabase(ctx.Entity);
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
            return new(ctx.Entity, _validator);
        }

        public Result<OrigamiBlog> Deactivate(DataOperationContext<OrigamiBlog> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.DeactivateBlogs));
                if (permission.Ok == false) return permission;
            }

            var fresh = ReadFromDatabase(ctx.Entity);
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
                var fresh = ReadFromDatabase(ctx.Entity);
                if (fresh == null)
                {
                    hub.Error = Text.Original("Blog could not be found");
                }
                else if (fresh is { IsPrimary: true })
                {
                    hub.Error = Text.Original("Primary blog cannot be deleted");
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

        public override Result<OrigamiBlog> Purge(DataOperationContext<OrigamiBlog> ctx)
        {
            var hub = new Result<OrigamiBlog>();

            using var db = DbContextFactory.CreateDbContext();

            this._purgeCategories(db, ctx).Push(hub);
            this._purgePages(db, ctx).Push(hub);
            this._purgePosts(db, ctx).Push(hub);
            this._purgeQuickNotes(db, ctx).Push(hub);
            this._purgeVideos(db, ctx).Push(hub);

            // blogs the users have access to
            db.UserBlogs.AsNoTracking().Where(x => x.BlogId == ctx.Entity.Id).ExecuteDelete();

            // blog is purged at last to prevent foreign key constraint issues
            base.Purge(ctx);

            return hub;
        }

        public override Result<OrigamiBlog> PurgeValidation(DataOperationContext<OrigamiBlog> ctx)
        {
            var hub = base.DeleteValidation(ctx);
            if (hub.Ok)
            {
                var fresh = ReadFromDatabase(ctx.Entity);
                if (fresh == null)
                {
                    hub.Error = Text.Original("Blog could not be found");
                }
                else if (fresh is { IsPrimary: true })
                {
                    hub.Error = Text.Original("Primary blog cannot be purged");
                }
            }
            return hub;
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

            //fresh from the oven
            var fresh = this.ReadFromDatabase(ctx.Entity)!;

            //pulls the latest version of the blog entity from the database to ensure the cache is updated with the correct version
            ctx.Entity.Version(fresh);

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

            using var db = DbContextFactory.CreateDbContext();

            var blogs = from i in ctx.Entity
                        join b in db.Set<OrigamiBlog>().AsNoTracking().NonDeleted().Active() on i equals b.Id
                        select new { b, i };

            if (ctx.Entity.Count() != blogs.Count())
            {
                return new() { Error = Text.Original("Invalid ids (may contain deleted, inactive blog ids)") };
            }

            try
            {
                var hub = new Result();
                var index = 0;
                foreach (var blog in blogs)
                {
                    if (hub.Ok == false) return hub;
                    var before = ReadFromDatabase(blog.b)!;
                    var update = ReadFromDatabase(blog.b)!;
                    update.Order = ++index;
                    SmartUpdate(new DataOperationContext<OrigamiBlog>(ctx.User, DateTime.UtcNow, update, before), false).Push(hub);
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

            using var db = DbContextFactory.CreateDbContext();

            var blogs = db.Set<OrigamiBlog>().AsNoTracking().NonDeleted().Active();

            try
            {
                var hub = new Result();

                foreach (var blog in blogs)
                {
                    if (hub.Ok == false) return hub;
                    var before = ReadFromDatabase(blog)!;
                    var update = ReadFromDatabase(blog)!;
                    update.Order = null;
                    SmartUpdate(new DataOperationContext<OrigamiBlog>(ctx.User, DateTime.UtcNow, update, before), false).Push(hub);
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
            return new(ctx.Entity, _validator);
        }
        private Result _purgeCategories(OrigamiDbContext db, DataOperationContext<OrigamiBlog> ctx)
        {
            var categories = from a in db.Categories.AsNoTracking().Blog(ctx.Entity.Id)
                             select new OrigamiCategory { Id = a.Id, NanoId = a.NanoId };

            if (categories.Any() == true)
            {
                categories.GetContexts(ctx).Each(_categoryRepository.Purge);
            }

            return new();
        }

        private Result _purgePages(OrigamiDbContext db, DataOperationContext<OrigamiBlog> ctx)
        {
            var pages = from p in db.Pages.AsNoTracking().Blog(ctx.Entity.Id)
                        select new OrigamiPage { Id = p.Id, NanoId = p.NanoId };

            if (pages.Any() == true)
            {
                pages.Select(x => _hubPageRepository.Get(x)).Each(x => _hubPageRepository.Purge(x, ctx.User));
            }

            return new();
        }

        private Result _purgePosts(OrigamiDbContext db, DataOperationContext<OrigamiBlog> ctx)
        {
            var posts = from p in db.Posts.AsNoTracking().Blog(ctx.Entity.Id)
                        select new OrigamiPost { Id = p.Id, NanoId = p.NanoId };

            if (posts.Any() == true)
            {
                posts.Select(x => _hubPostRepository.Get(x)).Each(x => _hubPostRepository.Purge(x, ctx.User));
            }

            return new();
        }

        private Result _purgeQuickNotes(OrigamiDbContext db, DataOperationContext<OrigamiBlog> ctx)
        {
            var quickNotes = from qn in db.QuickNotes.AsNoTracking().Blog(ctx.Entity.Id)
                             select new OrigamiQuickNote { Id = qn.Id, NanoId = qn.NanoId };

            if (quickNotes.Any() == true)
            {
                quickNotes.Select(x => _hubQuickNoteRepository.Get(x)).Each(x => _hubQuickNoteRepository.Purge(x, ctx.User));
            }

            return new();
        }

        private Result _purgeVideos(OrigamiDbContext db, DataOperationContext<OrigamiBlog> ctx)
        {
            var videos = from v in db.Videos.AsNoTracking().Blog(ctx.Entity.Id)
                         select new OrigamiVideo { Id = v.Id, NanoId = v.NanoId };

            if (videos.Any() == true)
            {
                videos.Select(x => _hubVideoRepository.Get(x)).Each(x => _hubVideoRepository.Purge(x, ctx.User));
            }

            return new();
        }
    }
}
