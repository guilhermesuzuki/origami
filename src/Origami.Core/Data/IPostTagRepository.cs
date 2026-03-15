using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPostTagRepository : IRepository<OrigamiPostTag>
    {
        Result RefreshCache(Guid blog, string previous, string current);
    }
}
