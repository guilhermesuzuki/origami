using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IWhatToSeeNextRepository
    {
        IEnumerable<OrigamiContent> GetWhatToSeeNext<T>(T entity) where T : ITitle, IContent, IId, new();
        IEnumerable<OrigamiContent> GetWhatToSeeNextTitle<T>(T entity) where T : ITitle, IContent, IId, new();
    }
}
