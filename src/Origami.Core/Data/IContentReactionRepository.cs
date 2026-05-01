using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IContentReactionRepository :
        IRepository<OrigamiContentReaction>,
        IReactions<OrigamiContent, OrigamiContentReaction>
    {

    }
}
