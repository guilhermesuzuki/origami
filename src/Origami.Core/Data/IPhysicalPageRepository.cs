using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPhysicalPageRepository : IRepository<OrigamiPhysicalPage>
    {
        Result<OrigamiPhysicalPageView> View<T>(string virtualPath, OrigamiPhysicalPageView view, T whoIsResponsible);
        long Views(string virtualPath);
    }
}
