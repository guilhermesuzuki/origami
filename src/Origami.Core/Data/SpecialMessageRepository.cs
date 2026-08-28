using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
            IAppFacade appFacade,
            IValidator<OrigamiSpecialMessage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
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

        public IEnumerable<OrigamiSpecialMessage> GetVisibleMessages()
        {
            var now = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            var messages = base.ReadFromCache();
            return messages.NonDeleted().Published().Where(x => x.StartDate <= now).Where(x => x.EndDate >= now);
        }

        public override Result<OrigamiSpecialMessage> CreateValidation(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiSpecialMessage> DeleteValidation(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiSpecialMessage> PurgeValidation(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiSpecialMessage> UpdateValidation(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        private Result<OrigamiSpecialMessage> _validationForAllOperations(DataOperationContext<OrigamiSpecialMessage> ctx)
        {
            Result<OrigamiSpecialMessage> result = new(ctx.Entity, _validator);
            result.Error = Text.Original("Operation not allowed");
            result.Error = Text.Original("Use the HubContentSpecialMessage repository instead");
            return result;
        }
    }
}
