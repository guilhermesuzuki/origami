using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserPasswordResetRepository :
        RepositoryOuterLayer<OrigamiUserPasswordReset>,
        IUserPasswordResetRepository
    {
        public UserPasswordResetRepository(
            Text text,
            IMyMemoryCache memoryCache,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override Result<OrigamiUserPasswordReset> CanCreate(DataOperationContext<OrigamiUserPasswordReset> ctx)
        {
            if (ctx.Entity.UserId == ctx.User.Id)
            {
                return CheckPermission(ctx, nameof(OrigamiRole.ResetOwnPassword));
            }

            return CheckPermission(ctx, nameof(OrigamiRole.ResetOtherUsersPasswords));
        }
    }
}
