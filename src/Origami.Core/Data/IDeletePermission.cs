using Origami.Core.Models;

namespace Origami.Core.Data;

public interface IDeletePermission<T>
{
    /// <summary>
    /// Delete other users <typeparamref name="T"/> permission name
    /// </summary>
    string DeleteOtherUsersPermission { get; }

    /// <summary>
    /// Delete own <typeparamref name="T"/> permission name
    /// </summary>
    string DeleteOwnPermission { get; }

    /// <summary>
    /// Delete <typeparamref name="T"/> permission name
    /// </summary>
    string DeletePermission { get; }

    /// <summary>
    /// Purge permission name
    /// </summary>
    string PurgePermission { get; }

    /// <summary>
    /// Restore permission name
    /// </summary>
    string RestorePermission { get; }

    /// <summary>
    /// Can the user delete, given their permissions and operation ctx?
    /// </summary>
    /// <param name="dataOperationContext"></param>
    /// <returns></returns>
    Result<T> CanDelete(DataOperationContext<T> dataOperationContext);

    /// <summary>
    /// Can the user purge, given their permissions and operation ctx?
    /// </summary>
    /// <param name="dataOperationContext"></param>
    /// <returns></returns>
    Result<T> CanPurge(DataOperationContext<T> dataOperationContext);

    /// <summary>
    /// Can the user restore, given their permissions and operation ctx?
    /// </summary>
    /// <param name="dataOperationContext"></param>
    /// <returns></returns>
    Result<T> CanRestore(DataOperationContext<T> dataOperationContext);
}
