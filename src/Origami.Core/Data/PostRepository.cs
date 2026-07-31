using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PostRepository : RepositoryOuterLayer<OrigamiPost>, IPostRepository
    {
        protected readonly IValidator<OrigamiPost> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostRepository(
            IAppFacade appFacade,
            IValidator<OrigamiPost> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewPosts);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPosts);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPosts);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersPosts);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnPosts);
        public override string PurgePermission => nameof(OrigamiRole.PurgePosts);
        public override string ReadPermission => nameof(OrigamiRole.ViewPosts);
        public override string RestorePermission => nameof(OrigamiRole.RestorePosts);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersPosts);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnPosts);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPosts);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPosts);

        public override Result<OrigamiPost> CreateValidation(DataOperationContext<OrigamiPost> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiPost> DeleteValidation(DataOperationContext<OrigamiPost> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiPost> PurgeValidation(DataOperationContext<OrigamiPost> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiPost> UpdateValidation(DataOperationContext<OrigamiPost> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        private Result<OrigamiPost> _validationForAllOperations(DataOperationContext<OrigamiPost> ctx)
        {
            Result<OrigamiPost> result = new(ctx.Entity);
            result.Error = Text.Original("Operation not allowed");
            result.Error = Text.Original("Use the HubContentPost repository instead");
            return result;
        }
    }
}
