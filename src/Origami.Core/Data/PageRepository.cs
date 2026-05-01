using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PageRepository : RepositoryOuterLayer<OrigamiPage>, IPageRepository
    {
        protected readonly IValidator<OrigamiPage> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PageRepository(
            IValidator<OrigamiPage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewPages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPages);
        public string MarkAsFrontPagePermission => nameof(OrigamiRole.PromoteToFrontPage);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersPages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnPages);
        public override string PurgePermission => nameof(OrigamiRole.PurgePages);
        public override string ReadPermission => nameof(OrigamiRole.ViewPages);
        public override string RestorePermission => nameof(OrigamiRole.RestorePages);
        public string UnmarkAsFrontPagePermission => nameof(OrigamiRole.DemoteFromFrontPage);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersPages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnPages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPages);

        public override Result<OrigamiPage> CreateValidation(DataOperationContext<OrigamiPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiPage> DeleteValidation(DataOperationContext<OrigamiPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiPage> PurgeValidation(DataOperationContext<OrigamiPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiPage> UpdateValidation(DataOperationContext<OrigamiPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        private Result<OrigamiPage> _validationForAllOperations(DataOperationContext<OrigamiPage> ctx)
        {
            Result<OrigamiPage> result = new(ctx.Entity);
            result.Error = Text.Original("Operation not allowed");
            result.Error = Text.Original("Use the HubContentPage repository instead");
            return result;
        }
    }
}
