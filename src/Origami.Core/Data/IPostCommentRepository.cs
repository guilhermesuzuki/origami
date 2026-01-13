using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPostCommentRepository :
        IRepository<OrigamiPostComment>,
        IComments<OrigamiPost, OrigamiPostComment>,
        IFastRead<PostCommentTotal>
    {

    }
}
