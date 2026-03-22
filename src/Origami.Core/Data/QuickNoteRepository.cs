using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class QuickNoteRepository : RepositoryOuterLayer<OrigamiQuickNote>, IQuickNoteRepository
    {
        protected readonly IValidator<OrigamiQuickNote> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public QuickNoteRepository(
            IValidator<OrigamiQuickNote> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewQuickNotes);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersQuickNotes);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnQuickNotes);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersQuickNotes);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnQuickNotes);
        public override string PurgePermission => nameof(OrigamiRole.PurgeQuickNotes);
        public override string ReadPermission => nameof(OrigamiRole.ViewQuickNotes);
        public override string RestorePermission => nameof(OrigamiRole.RestoreQuickNotes);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersQuickNotes);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnQuickNotes);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersQuickNotes);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnQuickNotes);

        public override Result<OrigamiQuickNote> CreateValidation(DataOperationContext<OrigamiQuickNote> ctx)
        {
            return new Result<OrigamiQuickNote>(ctx.Entity, _validator);
        }

        public override Result<OrigamiQuickNote> UpdateValidation(DataOperationContext<OrigamiQuickNote> ctx)
        {
            return new Result<OrigamiQuickNote>(ctx.Entity, _validator);
        }
    }
}
