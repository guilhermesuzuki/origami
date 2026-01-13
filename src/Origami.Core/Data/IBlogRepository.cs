using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IBlogRepository : IRepository<OrigamiBlog>
    {
        /// <summary>
        /// Activates a blog, given a <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiBlog> Activate(DataOperationContext<OrigamiBlog> ctx, bool checkPermission);

        /// <summary>
        /// Deactivates a blog, given a <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<OrigamiBlog> Deactivate(DataOperationContext<OrigamiBlog> ctx, bool checkPermission);

        /// <summary>
        ///  Directory where scaled images will be saved
        /// </summary>
        /// <returns></returns>
        string DirectoryForScalingImages();

        /// <summary>
        /// Returns the only primary blog
        /// </summary>
        /// <returns></returns>
        OrigamiBlog GetPrimary();

        /// <summary>
        /// Sets the primary blog in the specified data operation ctx.
        /// </summary>
        /// <param name="ctx">The data operation ctx containing the blog to be set as primary. Cannot be null.</param>
        /// <returns>A <see cref="Result{T}"/> containing the updated blog if the operation succeeds,  or an error result if the
        /// operation fails.</returns>
        Result<OrigamiBlog> SetPrimary(DataOperationContext<OrigamiBlog> ctx, bool checkPermission);

        /// <summary>
        /// Sorts the blogs, given the <paramref name="ids"/>
        /// </summary>
        /// <param name="ctx">ids of the blogs in order</param>
        /// <returns></returns>
        Result SortThem(DataOperationContext<IEnumerable<Guid>> ctx, bool checkPermission);

        /// <summary>
        /// Sorts the blogs with default
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result SortThemWithDefault(DataOperationContext ctx, bool checkPermission);
    }
}
