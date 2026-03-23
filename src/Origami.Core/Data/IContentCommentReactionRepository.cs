using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IContentCommentReactionRepository : 
        IRepository<OrigamiContentCommentReaction>,
        IReactions<OrigamiContent, OrigamiContentCommentReaction>
    {

    }
}
