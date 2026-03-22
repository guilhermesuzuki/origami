using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPhysicalPageViewRepository :
        IRepository<OrigamiPhysicalPageView>,
        IViews<OrigamiPhysicalPage>,
        IFastRead<PhysicalPageViewTotal>
    {
        long GetViews<T>(T entity) where T: IId;
    }
}
