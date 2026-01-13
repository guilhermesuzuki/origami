using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPublish<T>
    {
        /// <summary>
        /// Publishes the <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="checkPermission"></param>
        /// <returns></returns>
        Result<T> SmartPublish(DataOperationContext<T> ctx, bool checkPermission);

        /// <summary>
        /// Unpublishes the <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="checkPermission"></param>
        /// <returns></returns>
        Result<T> SmartUnpublish(DataOperationContext<T> ctx, bool checkPermission);
    }
}
