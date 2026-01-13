using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IWhatToSeeNextRepository
    {
        IEnumerable<BaseContent> GetWhatToSeeNext<T>(T entity) where T : ITitle, IContent, IId, new();
        IEnumerable<BaseContent> GetWhatToSeeNextTitle<T>(T entity) where T : ITitle, IContent, IId, new();
    }
}
