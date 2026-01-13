using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISmartRestore<T> where T : IId
    {
        Result<T> SmartRestore(DataOperationContext<T> ctx, bool checkPermission);
    }
}
