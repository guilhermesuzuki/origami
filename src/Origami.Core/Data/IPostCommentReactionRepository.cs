using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPostCommentReactionRepository :
        IBaseRepository<OrigamiPostCommentReaction>,
        IRead<OrigamiPostCommentReaction>,
        ICache<OrigamiPostCommentReaction>,
        ISmartPurge<OrigamiPostCommentReaction>,
        IMerge<OrigamiPostCommentReaction>,
        ISearch<OrigamiPostCommentReaction>,
        IReactions<OrigamiPostComment, OrigamiPostCommentReaction>
    {

    }
}
