using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentSoftwareReleaseRepository : HubContentRepository<OrigamiSoftwareRelease, HubContentSoftwareRelease>
    {
        public HubContentSoftwareReleaseRepository(
            IMyMemoryCache memoryCache,
            IValidator<HubContentSoftwareRelease> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewSoftwareReleases);
        public override string CreatePermission => nameof(OrigamiRole.CreateNewSoftwareReleases);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersSoftwareReleases);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnSoftwareReleases);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersSoftwareReleases);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnSoftwareReleases);
        public override string PurgePermission => nameof(OrigamiRole.PurgeSoftwareReleases);
        public override string RestorePermission => nameof(OrigamiRole.RestoreSoftwareReleases);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersSoftwareReleases);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnSoftwareReleases);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersSoftwareReleases);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnSoftwareReleases);
    }
}
