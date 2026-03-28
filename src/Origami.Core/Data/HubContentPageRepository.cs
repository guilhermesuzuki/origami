using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentPageRepository : HubContentRepository<OrigamiPage, HubContentPage>
    {
        public HubContentPageRepository(
            IMemoryCache memoryCache,
            IValidator<HubContentPage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }
        public override string ReadPermission => nameof(OrigamiRole.ViewPages);
        public override string CreatePermission => nameof(OrigamiRole.CreateNewPages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPages);
        public override string PurgePermission => nameof(OrigamiRole.PurgePages);
        public override string RestorePermission => nameof(OrigamiRole.RestorePages);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersPages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnPages);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersPages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnPages);
        public override string PromoteToFrontPagePermission => nameof(OrigamiRole.PromoteToFrontPage);
        public override string DemoteFromFrontPagePermission => nameof(OrigamiRole.DemoteFromFrontPage);
    }
}
