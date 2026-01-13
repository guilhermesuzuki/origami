using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IMerge<T>
    {
        Result Merge(DataOperationContext simple, (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) merge);
        Result MergeCache((IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) merge);
    }
}
