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

        public override string CreatePermission => nameof(OrigamiRole.CreateNewPages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPages);
        public override string PurgePermission => nameof(OrigamiRole.PurgePages);
        public override string RestorePermission => nameof(OrigamiRole.RestorePages);
    }
}
