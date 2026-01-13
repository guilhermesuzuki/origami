using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ICrud<T> :
        ICreate<T>,
        IRead<T>,
        IUpdate<T>,
        IDelete<T>,
        IPurge<T>,
        IRestore<T>
        where T : IId
    {

    }
}
