using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISpecialPageViewRepository :
        IRepository<OrigamiSpecialPageView>,
        IViews<OrigamiSpecialPage>,
        IFastRead<SpecialPageViewTotal>
    {

    }
}
