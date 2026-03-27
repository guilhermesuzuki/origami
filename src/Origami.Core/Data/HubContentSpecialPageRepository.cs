using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentSpecialPageRepository : HubContentRepository<OrigamiSpecialPage, HubContentSpecialPage>
    {
        public HubContentSpecialPageRepository(
            IMemoryCache memoryCache,
            IValidator<HubContentSpecialPage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewSpecialPages);
        public override string CreatePermission => nameof(OrigamiRole.CreateNewSpecialPages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersSpecialPages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnSpecialPages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersSpecialPages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnSpecialPages);
        public override string PurgePermission => nameof(OrigamiRole.PurgeSpecialPages);
        public override string RestorePermission => nameof(OrigamiRole.RestoreSpecialPages);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersSpecialPages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnSpecialPages);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersSpecialPages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnSpecialPages);
    }
}
