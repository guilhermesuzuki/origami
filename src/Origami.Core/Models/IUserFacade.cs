namespace Origami.Core.Models
{
    public interface IUserFacade :
        IId,
        IChanged,
        IBlogId
    {
        /// <summary>
        /// Event indicating that an Entity has changed
        /// </summary>
        event EventHandler<EntityOperation>? EntityHasChanged;

        /// <summary>
        /// Blogs the user has access to. This is used for the blog switcher in the admin area and for filtering content in the front-end. It should be set when the user logs in and whenever their permissions change.
        /// </summary>
        IEnumerable<OrigamiBlog> BlogsTheUserHasAccessTo { get; set; }

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
        OrigamiSocialProfile SocialProfile { get; set; }

        /// <summary>
        /// Current logged-in user (admin)
        /// </summary>
        OrigamiUser User { get; set; }

        /// <summary>
        /// Method that calls <see cref="EntityHasChanged"/> event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entityOperation"></param>
        void EntityChanged(object sender, EntityOperation entityOperation);
    }
}
