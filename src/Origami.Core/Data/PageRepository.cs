using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PageRepository : RepositoryOuterLayer<OrigamiPage>, IPageRepository
    {
        protected readonly IValidator<OrigamiPage> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PageRepository(
            IValidator<OrigamiPage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewPages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPages);
        public string MarkAsFrontPagePermission => nameof(OrigamiRole.PromoteToFrontPage);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersPages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnPages);
        public override string PurgePermission => nameof(OrigamiRole.PurgePages);
        public override string ReadPermission => nameof(OrigamiRole.ViewPages);
        public override string RestorePermission => nameof(OrigamiRole.RestorePages);
        public string UnmarkAsFrontPagePermission => nameof(OrigamiRole.DemoteFromFrontPage);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersPages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnPages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPages);

        public virtual Result<OrigamiPage> CanMarkAsFrontPage(DataOperationContext<OrigamiPage> ctx)
        {
            return CheckPermission(ctx, MarkAsFrontPagePermission);
        }

        public virtual Result<OrigamiPage> CanUnmarkAsFrontPage(DataOperationContext<OrigamiPage> ctx)
        {
            return CheckPermission(ctx, UnmarkAsFrontPagePermission);
        }

        public override Result<OrigamiPage> CreateValidation(DataOperationContext<OrigamiPage> ctx)
        {
            return new(ctx.Entity, _validator);
        }

        public Result<OrigamiPage> MarkAsFrontPage(DataOperationContext<OrigamiPage> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanMarkAsFrontPage(ctx);
                if (permission.Ok == false) return permission;
            }

            var page = ReadFromDatabase(ctx.Entity);
            if (page != null)
            {
                if (page.IsFrontPage)
                {
                    return new(ctx.Entity) { Error = Text.Original("Page is already front-page") };
                }

                if (page.ParentId != null)
                {
                    return new(ctx.Entity) { Error = Text.Original("Only a top-level page can become front-page") };
                }

                try
                {
                    using var db = DbContextFactory.CreateDbContext();
                    var hub = new Result<OrigamiPage>(ctx.Entity);
                    var row1 = db.Set<OrigamiPage>().Where(x => x.BlogId == ctx.Entity.BlogId).Where(x => x.IsFrontPage).ExecuteUpdate(x => x.SetProperty(y => y.IsFrontPage, false));
                    var row2 = db.Set<OrigamiPage>().Where(x => x.BlogId == ctx.Entity.BlogId).Where(x => x.Id == ctx.Entity.Id).ExecuteUpdate(x => x.SetProperty(y => y.IsFrontPage, true));

                    hub.RowsAffected += row2;
                    hub.RowsAffected += row2;

                    var old = ReadFromCache().Blog(ctx.Entity.BlogId).FirstOrDefault(page => page.IsFrontPage);
                    var neu = ReadFromCache().Id(ctx.Entity.Id);

                    if (old != null)
                    {
                        var update = ReadFromDatabase(old);
                        this.UpdateCache(update ?? new());
                    }
                    if (neu != null)
                    {
                        var update = ReadFromDatabase(neu);
                        this.UpdateCache(update ?? new());
                    }

                    return hub;
                }
                catch (Exception ex)
                {
                    return new(ctx.Entity, ex.GetMessage());
                }
            }
            return new(ctx.Entity) { Error = Text.Original("Page cannot be retrieved") };
        }

        public override Result<OrigamiPage> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiPage> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();
            //var del = db.Set<OrigamiPageView>().Where(x => x.PageId == ctx.Entity.Id).ExecuteDelete();
            return new Result<OrigamiPage>(ctx.Entity) { /*RowsAffected = del*/ };
        }

        public Result<OrigamiPage> UnmarkAsFrontPage(DataOperationContext<OrigamiPage> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanUnmarkAsFrontPage(ctx);
                if (permission.Ok == false) return permission;
            }

            var page = ReadFromDatabase(ctx.Entity);
            if (page != null)
            {
                if (page.IsFrontPage)
                {
                    try
                    {
                        using var db = DbContextFactory.CreateDbContext();
                        var hub = new Result<OrigamiPage>(ctx.Entity);
                        var row = db.Set<OrigamiPage>().Where(x => x.BlogId == ctx.Entity.BlogId).Where(x => x.IsFrontPage).ExecuteUpdate(x => x.SetProperty(y => y.IsFrontPage, false));
                        hub.RowsAffected = row;

                        var old = ReadFromCache().Blog(ctx.Entity.BlogId).FirstOrDefault(page => page.IsFrontPage);
                        if (old != null)
                        {
                            var update = ReadFromDatabase(old);
                            this.UpdateCache(update ?? new());
                        }

                        return hub;
                    }
                    catch (Exception ex)
                    {
                        return new(ctx.Entity, ex.GetMessage());
                    }
                }
                return new(ctx.Entity) { Error = Text.Original("Page is not front-page") };
            }
            return new(ctx.Entity) { Error = Text.Original("Page does not exist") };
        }

        public override Result<OrigamiPage> UpdateValidation(DataOperationContext<OrigamiPage> ctx)
        {
            return new(ctx.Entity, _validator);
        }
    }
}
