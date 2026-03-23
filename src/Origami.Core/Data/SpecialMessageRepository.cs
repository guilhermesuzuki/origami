using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class SpecialMessageRepository :
        RepositoryOuterLayer<OrigamiSpecialMessage>,
        ISpecialMessageRepository
    {
        protected readonly IValidator<OrigamiSpecialMessage> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SpecialMessageRepository(
            IValidator<OrigamiSpecialMessage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewSpecialMessages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersSpecialMessages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnSpecialMessages);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersSpecialMessages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnSpecialMessages);
        public override string PurgePermission => nameof(OrigamiRole.PurgeSpecialMessages);
        public override string ReadPermission => nameof(OrigamiRole.ViewSpecialMessages);
        public override string RestorePermission => nameof(OrigamiRole.RestoreSpecialMessages);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersSpecialMessages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnSpecialMessages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersSpecialMessages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnSpecialMessages);

        public override Result<OrigamiSpecialMessage> CreateValidation(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            return new Result<OrigamiSpecialMessage>(ctx.Entity, _validator);
        }

        public IEnumerable<OrigamiSpecialMessage> GetVisibleMessages()
        {
            var now = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            var messages = base.ReadFromCache();
            return messages.NonDeleted().Published().Where(x => x.StartDate <= now).Where(x => x.EndDate >= now);
        }

        public override Result<OrigamiSpecialMessage> UpdateValidation(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            return new Result<OrigamiSpecialMessage>(ctx.Entity, _validator);
        }
    }
}
