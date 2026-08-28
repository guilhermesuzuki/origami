using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IComments<T>
        where T : IId
    {
        /// <summary>
        /// Returns the number of comments from a <typeparamref name="T"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        long GetComments(T entity);
    }

    public interface IComments<T, TComment> :
        IComments<T>
        where T : IId
        where TComment : IId
    {
        /// <summary>
        /// Returns the list of comments from <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="deleted">returns the deleted ones</param>
        /// <returns></returns>
        List<TComment> AllComments(T? entity);

        /// <summary>
        /// Returns the list of comments from <paramref name="socialProfile"/>
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <param name="deleted">returns the deleted ones</param>
        /// <returns></returns>
        List<TComment> CommentsFromProfile(OrigamiSocialProfile socialProfile, bool deleted);

        /// <summary>
        /// Creates a new comment within the specified data operation ctx.
        /// </summary>
        /// <param name="ctx">The data operation ctx containing the necessary information to create the comment. This parameter
        /// cannot be null.</param>
        /// <returns>A <see cref="Result{TComment}"/> representing the outcome of the operation. The result contains the created
        /// comment if the operation is successful, or error details if it fails.</returns>
        Result<TComment> SmartCreate(DataOperationContextFrontEnd<TComment> ctx);

        /// <summary>
        /// Deletes a comment from the data store based on the provided operation ctx.
        /// </summary>
        /// <remarks>The delete operation is performed within the scope of the provided ctx, which may
        /// include additional constraints or conditions. Ensure that the ctx is properly configured before
        /// invoking this method.</remarks>
        /// <param name="ctx">The operation ctx containing the comment to be deleted and any additional metadata required for the
        /// operation. This parameter cannot be null.</param>
        /// <returns>A <see cref="Result{TComment}"/> indicating the outcome of the delete operation. The result contains the
        /// deleted comment if the operation is successful, or an error  state if the operation fails.</returns>
        Result<TComment> SmartDelete(DataOperationContextFrontEnd<TComment> ctx);

        /// <summary>
        /// Pins the specified comment within the given data operation ctx.
        /// </summary>
        /// <remarks>This method is used to mark a comment as pinned within the provided ctx.  The
        /// operation's success or failure is encapsulated in the returned <see cref="Result{TComment}"/>
        /// object.</remarks>
        /// <param name="ctx">The data operation ctx containing the comment to be pinned.  This parameter must not be <see
        /// langword="null"/>.</param>
        /// <returns>A <see cref="Result{TComment}"/> object representing the outcome of the pin operation.  The result contains
        /// the pinned comment if the operation is successful.</returns>
        Result<TComment> Pin(DataOperationContextFrontEnd<TComment> ctx);

        Result<TComment> Pin(DataOperationContext<TComment> ctx, bool checkPermission);

        /// <summary>
        /// Removes the pinned status from a comment in the specified ctx.
        /// </summary>
        /// <remarks>Use this method to unpin a comment that was previously marked as pinned.  The
        /// operation ctx should include all necessary information to identify and process the comment.</remarks>
        /// <param name="ctx">The operation ctx containing the comment to unpin. This parameter must not be null.</param>
        /// <returns>A <see cref="Result{TComment}"/> representing the outcome of the operation.  If successful, the result
        /// contains the updated comment with its pinned status removed.</returns>
        Result<TComment> Unpin(DataOperationContextFrontEnd<TComment> ctx);

        Result<TComment> Unpin(DataOperationContext<TComment> ctx, bool checkPermission);

        /// <summary>
        /// Updates an existing comment in the data store based on the provided ctx.
        /// </summary>
        /// <remarks>The update operation modifies an existing comment in the data store. Ensure that the
        /// ctx  includes a valid comment object and any necessary identifiers or metadata required for the
        /// update.</remarks>
        /// <param name="ctx">The operation ctx containing the comment to update and any additional metadata required for the
        /// operation. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="Result{TComment}"/> object representing the outcome of the update operation.  If successful,
        /// the result contains the updated comment; otherwise, it contains error details.</returns>
        Result<TComment> SmartUpdate(DataOperationContextFrontEnd<TComment> ctx);
    }
}
