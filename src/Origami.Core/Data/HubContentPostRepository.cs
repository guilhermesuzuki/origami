using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentPostRepository : HubContentRepository<OrigamiPost, HubContentPost>
    {
        public HubContentPostRepository(
            IMemoryCache memoryCache,
            IValidator<HubContentPost> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewPosts);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPosts);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPosts);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPosts);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPosts);
        public override string PurgePermission => nameof(OrigamiRole.PurgePosts);
        public override string RestorePermission => nameof(OrigamiRole.RestorePosts);
    }
}
