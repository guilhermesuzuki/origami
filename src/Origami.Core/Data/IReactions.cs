using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IReactions<T, TReaction> where TReaction : IId
    {
        /// <summary>
        /// Returns all reactions from a <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IEnumerable<TReaction> Reactions(T entity);

        /// <summary>
        /// Returns all reactions from a <paramref name="socialProfile"/>
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <returns></returns>
        IEnumerable<TReaction> ReactionsFromProfile(OrigamiSocialProfile socialProfile);

        /// <summary>
        /// Creates a new instance of the specified reaction type within the given data operation ctx.
        /// </summary>
        /// <remarks>The method ensures that the reaction is created in accordance with the rules and
        /// constraints defined  by the provided <paramref name="ctx"/>. The caller should verify the result to
        /// determine whether  the operation was successful.</remarks>
        /// <param name="ctx">The data operation ctx that provides the necessary information and resources for creating the reaction.
        /// This parameter cannot be null.</param>
        /// <returns>A <see cref="Result{TReaction}"/> object containing the created reaction instance if successful,  or an
        /// error result if the operation fails.</returns>
        Result<TReaction> SmartCreate(DataOperationContextFrontEnd<TReaction> ctx);

        /// <summary>
        /// Removes all data associated with the specified operation ctx.
        /// </summary>
        /// <remarks>Use this method to clean up data associated with a specific operation ctx.  The
        /// exact behavior of the purge operation depends on the implementation of the  <typeparamref name="TReaction"/>
        /// type and the provided ctx.</remarks>
        /// <param name="ctx">The operation ctx that defines the scope of the data to be purged.  This parameter cannot be null.</param>
        /// <returns>A <see cref="Result{TReaction}"/> object representing the outcome of the purge operation,  including any
        /// relevant reaction or status information.</returns>
        Result<TReaction> SmartPurge(DataOperationContextFrontEnd<TReaction> ctx);
    }
}
