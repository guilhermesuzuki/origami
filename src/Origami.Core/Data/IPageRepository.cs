using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPageRepository : IRepository<OrigamiPage>, IPublish<OrigamiPage>
    {
        
    }
}
