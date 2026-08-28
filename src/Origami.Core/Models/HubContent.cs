namespace Origami.Core.Models
{
    public abstract class HubContent<T> :
        IHubContent<T>,
        IAuthorId,
        IHeaderImage
        where T : OrigamiContent
    {
        protected HubContent() { }

        public Guid AuthorId
        {
            get => Entity.AuthorId;
            set => Entity.AuthorId = value;
        }

        public Guid? BlogId { get => Entity.BlogId; set => Entity.BlogId = value; }

        public List<OrigamiContentCategory> Categories { get; set; } = [];

        public List<T> Children { get; set; } = [];

        public List<OrigamiContentComment> Comments { get; set; } = [];

        /// <summary>
        /// The main entity, root of all information here
        /// </summary>
        public T Entity { get; set; } = Activator.CreateInstance<T>();

        /// <summary>
        /// Header image for the content, if any. This is a URL or path to the image.
        /// </summary>
        public string HeaderImage { get => Entity.HeaderImage; set => Entity.HeaderImage = value; }

        public List<OrigamiContentHistory> Histories { get; set; } = [];

        /// <summary>
        /// Dummy implementation to satisfy IHubContent interface, since the actual ID is stored in the Entity
        /// </summary>
        public Guid Id { get => Entity.Id; set => Entity.Id = value; }

        /// <summary>
        /// Necessary implementation of the INanoId interface, since the actual NanoId is stored in the Entity
        /// </summary>
        public string NanoId => Entity.NanoId;

        public T? Parent { get; set; }
        public List<OrigamiContentRating> Ratings { get; set; } = [];
        public List<OrigamiContentReaction> Reactions { get; set; } = [];
        public List<OrigamiContentTag> Tags { get; set; } = [];
    }
}
