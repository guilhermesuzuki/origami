namespace Origami.Core.Models
{
    public interface IUserFacade :
        IId,
        IChanged,
        IBlogId,
        IDisposable
    {
        event EventHandler<string> RefreshingTheUI;

        /// <summary>
        /// Blogs the user has access to. This is used for the blog switcher in the admin area and for filtering content in the front-end. It should be set when the user logs in and whenever their permissions change.
        /// </summary>
        IEnumerable<OrigamiBlog> BlogsTheUserHasAccessTo { get; }

        /// <summary>
        /// Is the application in incognito mode?
        /// </summary>
        bool IncognitoMode { get; set; }

        /// <summary>
        /// Process result for CRUD and other types of Operation
        /// </summary>
        Result Result { set; }

        /// <summary>
        /// Process results in memory
        /// </summary>
        IList<Result> Results { get; }

        /// <summary>
        /// Gets or sets the search query string used to filter or locate specific items.
        /// </summary>
        string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cookie consent banner should be displayed to the user.
        /// </summary>
        bool ShowCookieConsent { get; set; }

        /// <summary>
        /// Current logged-in user (front-end)
        /// </summary>
        OrigamiSocialProfile SocialProfile { get; }

        /// <summary>
        /// Gets or sets the unique identifier of the social profile associated with the current logged-in user.
        /// </summary>
        Guid SocialProfileId { get; set; }

        /// <summary>
        /// Current logged-in user (admin)
        /// </summary>
        OrigamiUser User { get; }

        /// <summary>
        /// Gets or sets the unique identifier of the user associated with the current logged-in user.
        /// </summary>
        Guid UserId { get; set; }

        /// <summary>
        /// Triggers the RefreshingTheUI event to notify subscribers that the user interface should be refreshed for a specific key.
        /// </summary>
        /// <param name="key"></param>
        void RefreshTheUI(string key);
    }
}
