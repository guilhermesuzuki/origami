using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentSpecialMessageRepository : HubContentRepository<OrigamiSpecialMessage, HubContentSpecialMessage>
    {
        public HubContentSpecialMessageRepository(
            IMyMemoryCache memoryCache,
            IValidator<HubContentSpecialMessage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewSpecialMessages);
        public override string CreatePermission => nameof(OrigamiRole.CreateNewSpecialMessages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersSpecialMessages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnSpecialMessages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersSpecialMessages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnSpecialMessages);
        public override string PurgePermission => nameof(OrigamiRole.PurgeSpecialMessages);
        public override string RestorePermission => nameof(OrigamiRole.RestoreSpecialMessages);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersSpecialMessages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnSpecialMessages);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersSpecialMessages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnSpecialMessages);
    }
}
