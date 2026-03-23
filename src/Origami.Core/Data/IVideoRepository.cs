using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IVideoRepository : IRepository<OrigamiVideo>, IPublish<OrigamiVideo>
    {

    }
}
