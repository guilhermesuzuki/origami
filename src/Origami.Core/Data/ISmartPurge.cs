using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISmartPurge<T> where T : IId
    {
        Result<T> SmartPurge(DataOperationContext<T> ctx, bool checkPermission);
    }
}
