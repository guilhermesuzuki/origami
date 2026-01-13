using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPageViewRepository :
        IRepository<OrigamiPageView>,
        IViews<OrigamiPage>,
        IFastRead<PageViewTotal>
    {

    }
}
