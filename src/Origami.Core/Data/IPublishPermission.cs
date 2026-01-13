using Origami.Core.Models;

namespace Origami.Core.Data;

public interface IPublishPermission<T>
{
    /// <summary>
    /// Publish own <typeparamref name="T"/> permission name
    /// </summary>
    string PublishOwnPermission { get; }

    /// <summary>
    /// Publish other users <typeparamref name="T"/> permission name
    /// </summary>
    string PublishOtherUsersPermission { get; }

    /// <summary>
    /// Unpublish own <typeparamref name="T"/> permission name
    /// </summary>
    string UnpublishOwnPermission { get; }

    /// <summary>
    /// Unpublish other users <typeparamref name="T"/> permission name
    /// </summary>
    string UnpublishOtherUsersPermission { get; }

    /// <summary>
    /// Can the user publish, given their permissions and operation ctx?
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    Result<T> CanPublish(DataOperationContext<T> ctx);

    /// <summary>
    /// Can the user unpublish, given their permissions and operation ctx?
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    Result<T> CanUnpublish(DataOperationContext<T> ctx);
}
