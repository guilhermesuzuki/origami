using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ICache<T> : ICreateCache<T>, IReadCache<T>, IUpdateCache<T>, IDeleteCache<T>, IPurgeCache<T>
        where T : IId
    {

    }
}
