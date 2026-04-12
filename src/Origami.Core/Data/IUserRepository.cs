using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IUserRepository : IRepository<OrigamiUser>
    {
        /// <summary>
        /// Blocks the specified user within the given data operation context.
        /// </summary>
        /// <param name="ctx">The data operation context containing the user to be blocked. Cannot be null.</param>
        /// <param name="checkPermission">A boolean value indicating whether to check for necessary permissions before blocking the user. If <see
        /// langword="true"/>, permissions are checked; otherwise, they are not.</param>
        /// <returns>A <see cref="Result{OrigamiUser}"/> indicating the outcome of the block operation, including the user
        /// details if successful.</returns>
        Result<OrigamiUser> Block(DataOperationContext<OrigamiUser> ctx, bool checkPermission);

        /// <summary>
        /// Determines whether the specified user has permission to moderate comments.
        /// </summary>
        /// <param name="user">The user whose moderation permissions are being evaluated. Cannot be null.</param>
        /// <returns>true if the user is authorized to moderate comments; otherwise, false.</returns>
        bool CanTheUserModerateComments(IId user);

        /// <summary>
        /// Changes the user password, validating the old and the new one
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="newPassword1"></param>
        /// <param name="newPassword2"></param>
        /// <returns></returns>
        Result<OrigamiUser> ChangePassword(DataOperationContext<OrigamiUser> ctx, string oldPassword, string newPassword1, string newPassword2);

        /// <summary>
        /// Logged-in user forgot their password, system will generate a new one and return to them
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>a clean and new temporary password</returns>
        Result<string> ForgotOwnPassword(DataOperationContext<OrigamiUser> ctx, bool checkPermission);

        /// <summary>
        /// Looks up a user with a password in the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="cleanPassword"></param>
        /// <returns></returns>
        OrigamiUser? LookupUserInDatabase(string username, string cleanPassword);

        /// <summary>
        /// Resets the 2FA for a user, forcing it to go through the 2FA setup process again. This is typically used when a user has lost access to their 2FA device or needs to reconfigure their 2FA settings for security reasons.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="checkPermission"></param>
        /// <returns></returns>
        Result Reset2FA(DataOperationContext<OrigamiUser> ctx, bool checkPermission);

        /// <summary>
        /// Logged-out user forgot their password, system will generate a link to reset it
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<string> ResetPassword(DataOperationContext<OrigamiUser> ctx, bool checkPermission);

        /// <summary>
        /// Resets the password for a specified user.
        /// </summary>
        /// <param name="ctx">The data operation context containing the user for whom the password is being reset.</param>
        /// <param name="key">A unique key used to authorize the password reset operation.</param>
        /// <param name="newPassword1">The new password to set for the user.</param>
        /// <param name="newPassword2">A confirmation of the new password, which must match <paramref name="newPassword1"/>.</param>
        /// <param name="checkPermission">A boolean value indicating whether to check user permissions before resetting the password. <see
        /// langword="true"/> to check permissions; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="Result"/> indicating the success or failure of the password reset operation.</returns>
        Result ResetPassword(DataOperationContext<OrigamiUser> ctx, string key, string newPassword1, string newPassword2, bool checkPermission);
        /// <summary>
        /// Unblocks a user in the specified data operation context.
        /// </summary>
        /// <param name="ctx">The data operation context containing the user to be unblocked. Cannot be null.</param>
        /// <param name="checkPermission">If <see langword="true"/>, checks whether the current user has permission to unblock the user; otherwise,
        /// bypasses the permission check.</param>
        /// <returns>A <see cref="Result{OrigamiUser}"/> containing the unblocked user if successful; otherwise, an error result.</returns>
        Result<OrigamiUser> Unblock(DataOperationContext<OrigamiUser> ctx, bool checkPermission);
    }
}
