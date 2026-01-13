using Origami.Core.Models;

namespace Origami.Core.Data;

public interface IReadPermission<T>
{
    /// <summary>
    /// Read permission name
    /// </summary>
    string ReadPermission { get; }

    /// <summary>
    /// Can the user read <typeparamref name="T"/>s, given their permission?
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Result CanRead(Guid userId);
}
