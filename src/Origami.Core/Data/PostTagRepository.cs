using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PostTagRepository :
        RepositoryOuterLayer<OrigamiPostTag>,
        IPostTagRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostTagRepository(
            Text text,
            IMemoryCache memoryCache,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string DeletePermission => nameof(OrigamiRole.DeleteTags);
        public override string ReadPermission => nameof(OrigamiRole.ViewTags);
        public override string UpdatePermission => nameof(OrigamiRole.EditTags);

        public Result Delete(DataOperationContext<string> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var newContext = new DataOperationContext<OrigamiPostTag>(ctx.User, new() { Tag = ctx.Entity });
                var permission = CanDelete(newContext);
                if (permission.Ok == false) return permission;
            }
            return ReadFromDatabase().Where(x => x.Tag == ctx.Entity).GetContexts(ctx).Call(this.SmartDelete, false);
        }

        public Result Update(DataOperationContext<string> ctx, string tagToBeUpdated, bool checkPermission)
        {
            if (checkPermission)
            {
                var newContext = new DataOperationContext<OrigamiPostTag>(ctx.User, new() { Tag = ctx.Entity });
                var permission = CanUpdate(newContext);
                if (permission.Ok == false) return permission;
            }
            return ReadFromDatabase().Where(x => x.Tag == tagToBeUpdated).GetContexts(ctx).Call(this.SmartUpdate, false);
        }
    }
}
