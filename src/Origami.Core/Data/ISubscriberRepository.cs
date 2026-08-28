using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISubscriberRepository : IRepository<OrigamiSubscriber>
    {
        /// <summary>
        /// Subscribes the <paramref name="ctx"/> in the <paramref name="blog"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Result<OrigamiSubscriber> Subscribe(DataOperationContext<OrigamiSocialProfile> ctx, string email);

        /// <summary>
        /// Unsubscribes a user from the specified social profile.
        /// </summary>
        /// <param name="ctx">The context containing the social profile from which the user will be unsubscribed.</param>
        /// <param name="checkPermission">A value indicating whether to check for user permissions before unsubscribing.          <see
        /// langword="true"/> to check permissions; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="Result{OrigamiSubscriber}"/> indicating the outcome of the unsubscribe operation.          The
        /// result contains the subscriber information if the operation is successful.</returns>
        Result<OrigamiSubscriber> Unsubscribe(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission);

        /// <summary>
        /// Unsubscribes the specified social profile from receiving further updates.
        /// </summary>
        /// <remarks>This method removes the specified social profile from the list of active subscribers,
        /// preventing it from receiving future updates. Ensure that the context provided is valid and contains the
        /// necessary profile information.</remarks>
        /// <param name="ctx">The context containing the social profile to be unsubscribed.</param>
        /// <returns>A <see cref="Result{OrigamiSubscriber}"/> indicating the success or failure of the operation, along with the
        /// updated subscriber information.</returns>
        Result<OrigamiSubscriber> Unsubscribe(DataOperationContext<OrigamiSocialProfile> ctx);

        /// <summary>
        /// Validates the verification code for the subscriber
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool ValidateVerificationCode(DataOperationContext<OrigamiSocialProfile> ctx, string code);

        /// <summary>
        /// Attaches the verification code to the subscriber
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool VerificationCode(DataOperationContext<OrigamiSocialProfile> ctx, string email, string code);
    }
}
