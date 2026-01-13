using Origami.Core.Models;

namespace Origami.Core.Data;

public interface ICreatePermission<T>
{
    /// <summary>
    /// Create permission name
    /// </summary>
    string CreatePermission { get; }

    /// <summary>
    /// Can the user create a new <typeparamref name="T"/>, given their permissions and operation ctx?
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    Result<T> CanCreate(DataOperationContext<T> ctx);
}
