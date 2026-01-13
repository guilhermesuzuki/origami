using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISocialProfileRepository : IRepository<OrigamiSocialProfile>
    {
        /// <summary>
        /// Blocks the profile from a given <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiSocialProfile> Block(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission);

        /// <summary>
        /// Unblocks the profile from a given <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiSocialProfile> Unblock(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission);

        /// <summary>
        /// Grants moderator permissions to a profile, given a <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiSocialProfile> GrantModerator(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission);

        /// <summary>
        /// Revokes moderator permissions from a profile, given a <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiSocialProfile> RevokeModerator(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission);
    }
}
