using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IMerge<T>
    {
        Result Merge(DataOperationContext simple, Merge<T> merge);
        Result MergeCache(Merge<T> merge);
    }
}
