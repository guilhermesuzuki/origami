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
        string Search { get; set; }

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
