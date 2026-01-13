using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPurgeValidation<T> where T : IId
    {
        Result<T> PurgeValidation(DataOperationContext<T> ctx);
    }
}
