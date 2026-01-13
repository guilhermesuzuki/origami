using Origami.Core.Models;

namespace Origami.Core.Data;

public interface IUpdatePermission<T>
{
    /// <summary>
    /// Update <typeparamref name="T"/> permission name
    /// </summary>
    string UpdatePermission { get; }

    /// <summary>
    /// Update own <typeparamref name="T"/> permission name
    /// </summary>
    string UpdateOwnPermission { get; }

    /// <summary>
    /// Update other users <typeparamref name="T"/> permission name
    /// </summary>
    string UpdateOtherUsersPermission { get; }

    /// <summary>
    /// Can the user update, given their permissions and operation ctx?
    /// </summary>
    /// <param name="dataOperationContext"></param>
    /// <returns></returns>
    Result<T> CanUpdate(DataOperationContext<T> dataOperationContext);
}
