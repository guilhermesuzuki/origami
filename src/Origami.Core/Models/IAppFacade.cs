namespace Origami.Core.Models
{
    public interface IAppFacade : IAdmin
    {
        event EventHandler<object>? RefreshingTheUI;

        /// <summary>
        /// Environment name the application is running in.
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// One-time master password in SHA256 format for secure access.
        /// </summary>
        string OneTimeMasterPasswordInSHA256 { get; set; }

        /// <summary>
        /// List of online users in the application.
        /// </summary>
        IList<string> OnlineUsers { get; }

        /// <summary>
        /// Refresh the UI for a specific key in a global event.
        /// </summary>
        /// <param name="key"></param>
        void RefreshUI(string key);

        /// <summary>
        /// Refreshes the UI for a specific key in a global event with additional data.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void RefreshUI(string key, object data);

        /// <summary>
        /// Refreshes the user interface for the specified connection and key.
        /// </summary>
        /// <param name="connectionId">The unique identifier of the connection whose user interface should be refreshed. Cannot be null or empty.</param>
        /// <param name="key">The key representing the specific UI element or context to refresh. Cannot be null or empty.</param>
        void RefreshUI(string connectionId, string key);

        /// <summary>
        /// Refreshes the user interface for the specified connection and key with additional data.
        /// </summary>
        /// <param name="connectionId">The unique identifier of the connection whose user interface should be refreshed. Cannot be null or empty.</param>
        /// <param name="key">The key representing the specific UI element or context to refresh. Cannot be null or empty.</param>
        /// <param name="data">The additional data to include in the refresh event.</param>
        void RefreshUI(string connectionId, string key, object data);
    }
}
