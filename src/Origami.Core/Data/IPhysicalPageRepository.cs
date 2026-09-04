using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPhysicalPageRepository : IRepository<OrigamiPhysicalPage>
    {
        Result<OrigamiPhysicalPageView> View(string virtualPath, OrigamiPhysicalPageView view, OrigamiSocialProfile socialProfile);
        Result<OrigamiPhysicalPageView> View(string virtualPath, OrigamiPhysicalPageView view, OrigamiUser user);
    }
}
