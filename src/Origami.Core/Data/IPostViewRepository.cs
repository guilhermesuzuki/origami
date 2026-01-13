using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPostViewRepository :
        IRepository<OrigamiPostView>,
        IViews<OrigamiPost>,
        IFastRead<PostViewTotal>
    {

    }
}
