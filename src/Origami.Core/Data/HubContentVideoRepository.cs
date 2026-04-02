using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentVideoRepository : HubContentRepository<OrigamiVideo, HubContentVideo>
    {
        public HubContentVideoRepository(
            IMemoryCache memoryCache,
            IValidator<HubContentVideo> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewVideos);
        public override string CreatePermission => nameof(OrigamiRole.CreateNewVideos);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersVideos);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnVideos);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersVideos);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnVideos);
        public override string PurgePermission => nameof(OrigamiRole.PurgeVideos);
        public override string RestorePermission => nameof(OrigamiRole.RestoreVideos);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersVideos);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnVideos);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersVideos);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnVideos);
    }
}
