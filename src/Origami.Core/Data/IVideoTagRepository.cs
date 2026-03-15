using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IVideoTagRepository : IRepository<OrigamiVideoTag>
    {
        Result RefreshCache(Guid blog, string previous, string current);
    }
}
