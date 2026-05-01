namespace Origami.Core.Models
{
    /// <summary>
    /// Hub interface for content entities, which includes categories and tags.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHubContent<T> : IHub<T> where T : OrigamiContent
    {
        /// <summary>
        /// Gets or sets the parent element of the current object.
        /// </summary>
        T? Parent { get; set; }

        /// <summary>
        /// Gets or sets the collection of child elements of this instance.
        /// </summary>
        List<T> Children { get; set; }

        /// <summary>
        /// Categories associated with the entity
        /// </summary>
        List<OrigamiContentCategory> Categories { get; set; }

        /// <summary>
        /// Comments associated with the entity
        /// </summary>
        List<OrigamiContentComment> Comments { get; set; }

        /// <summary>
        /// Ratings associated with the entity
        /// </summary>
        List<OrigamiContentRating> Ratings { get; set; }

        /// <summary>
        /// Reactions associated with the entity
        /// </summary>
        List<OrigamiContentReaction> Reactions { get; set; }

        /// <summary>
        /// Tags associated with the entity
        /// </summary>
        List<OrigamiContentTag> Tags { get; set; }

        /// <summary>
        /// Histories associated with the entity
        /// </summary>
        List<OrigamiContentHistory> Histories { get; set; }
    }
}
