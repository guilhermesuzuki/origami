using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class SocialProfileRepository :
        RepositoryOuterLayer<OrigamiSocialProfile>,
        ISocialProfileRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SocialProfileRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewSocialProfiles);

        public Result<OrigamiSocialProfile> Block(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.BlockSocialProfiles));
                if (permission.Ok == false) return permission;
            }
            ctx.Entity.IsBlocked = true;
            return SmartUpdate(ctx, false);
        }

        public Result<OrigamiSocialProfile> GrantModerator(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.TurnSocialProfilesIntoModerators));
                if (permission.Ok == false) return permission;
            }
            ctx.Entity.IsModerator = true;
            return SmartUpdate(ctx, false);
        }

        public Result<OrigamiSocialProfile> RevokeModerator(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.RevokeModeratorRolesFromSocialProfiles));
                if (permission.Ok == false) return permission;
            }
            ctx.Entity.IsModerator = false;
            return SmartUpdate(ctx, false);
        }

        public Result<OrigamiSocialProfile> Unblock(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.UnblockSocialProfiles));
                if (permission.Ok == false) return permission;
            }
            ctx.Entity.IsBlocked = false;
            return SmartUpdate(ctx, false);
        }
    }
}
