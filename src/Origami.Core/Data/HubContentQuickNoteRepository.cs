using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class HubContentQuickNoteRepository : HubContentRepository<OrigamiQuickNote, HubContentQuickNote>
    {
        public HubContentQuickNoteRepository(
            IMemoryCache memoryCache,
            IValidator<HubContentQuickNote> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text
            ) : base(memoryCache, validator, dbContextFactory, text)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewQuickNotes);
        public override string CreatePermission => nameof(OrigamiRole.CreateNewQuickNotes);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersQuickNotes);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnQuickNotes);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersQuickNotes);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnQuickNotes);
        public override string PurgePermission => nameof(OrigamiRole.PurgeQuickNotes);
        public override string RestorePermission => nameof(OrigamiRole.RestoreQuickNotes);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersQuickNotes);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnQuickNotes);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersQuickNotes);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnQuickNotes);
    }
}
