using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IContentTagRepository : IRepository<OrigamiContentTag>
    {
        Result RefreshCache(Guid blog, string before, string current);
    }
}
