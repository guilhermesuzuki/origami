using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPhysicalPageViewRepository :
        IRepository<OrigamiPhysicalPageView>,
        IViews<OrigamiPhysicalPage>,
        IFastRead<PhysicalPageViewTotal>
    {

    }
}
