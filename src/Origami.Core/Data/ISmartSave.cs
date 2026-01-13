using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISmartSave<T> where T : IId
    {
        Result<T> SmartSave(DataOperationContext<T> ctx, bool checkPermission);
    }
}
