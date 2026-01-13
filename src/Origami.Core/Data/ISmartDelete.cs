using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISmartDelete<T> where T : IId
    {
        Result<T> SmartDelete(DataOperationContext<T> ctx, bool checkPermission);
    }
}
