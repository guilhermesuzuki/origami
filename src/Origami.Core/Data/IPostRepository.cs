using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPostRepository : IRepository<OrigamiPost>, IPublish<OrigamiPost>
    {

    }
}
