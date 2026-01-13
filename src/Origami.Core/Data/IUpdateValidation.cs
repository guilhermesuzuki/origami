using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IUpdateValidation<T> where T : IId
    {
        Result<T> UpdateValidation(DataOperationContext<T> ctx);
    }
}
