using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ICreateValidation<T> where T : IId
    {
        Result<T> CreateValidation(DataOperationContext<T> ctx);
    }
}
