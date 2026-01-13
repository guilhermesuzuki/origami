using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IDeleteValidation<T> where T : IId
    {
        Result<T> DeleteValidation(DataOperationContext<T> ctx);
    }
}
