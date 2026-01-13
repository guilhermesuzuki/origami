using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISocialProfileDeleteRepository : IRepository<OrigamiSocialProfileDelete>
    {
        /// <summary>
        /// Wipes ALL the user's data out from the website (purging reactions to comments in general, comments in videos and posts and ratings).
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="checkPermission"></param>
        /// <returns></returns>
        Result<OrigamiSocialProfileDelete> WipeDataOut(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission);
    }
}
