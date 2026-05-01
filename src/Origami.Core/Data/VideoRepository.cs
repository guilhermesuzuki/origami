using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoRepository :
        RepositoryOuterLayer<OrigamiVideo>,
        IVideoRepository
    {
        protected readonly IValidator<OrigamiVideo> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoRepository(
            IValidator<OrigamiVideo> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewVideos);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersVideos);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnVideos);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersVideos);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnVideos);
        public override string PurgePermission => nameof(OrigamiRole.PurgeVideos);
        public override string ReadPermission => nameof(OrigamiRole.ViewVideos);
        public override string RestorePermission => nameof(OrigamiRole.RestoreVideos);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersVideos);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnVideos);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersVideos);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnVideos);

        public override Result<OrigamiVideo> CreateValidation(DataOperationContext<OrigamiVideo> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiVideo> DeleteValidation(DataOperationContext<OrigamiVideo> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiVideo> PurgeValidation(DataOperationContext<OrigamiVideo> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiVideo> UpdateValidation(DataOperationContext<OrigamiVideo> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        private Result<OrigamiVideo> _validationForAllOperations(DataOperationContext<OrigamiVideo> ctx)
        {
            Result<OrigamiVideo> result = new(ctx.Entity, _validator);
            result.Error = Text.Original("Operation not allowed");
            result.Error = Text.Original("Use the HubContentVideo repository instead");
            return result;
        }
    }
}
