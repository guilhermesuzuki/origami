using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IRoleRepository : IRepository<OrigamiRole>
    {
        bool CanTheUserViewTheConnectivityDetails(OrigamiUser user);
    }
}
