using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IVideoCommentReactionRepository :
        IBaseRepository<OrigamiVideoCommentReaction>,
        IRead<OrigamiVideoCommentReaction>,
        ICache<OrigamiVideoCommentReaction>,
        ISmartPurge<OrigamiVideoCommentReaction>,
        IMerge<OrigamiVideoCommentReaction>,
        ISearch<OrigamiVideoCommentReaction>,
        IReactions<OrigamiVideoComment, OrigamiVideoCommentReaction>
    {

    }
}
