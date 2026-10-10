namespace Origami.Core.Models
{
    public abstract class HubContent : 
        IId,
        INanoId,
        IHeaderImage
    {
        public List<OrigamiContentCategory> Categories { get; set; } = [];

        public List<OrigamiContentComment> Comments { get; set; } = [];

        /// <summary>
        /// Header image for the content, if any. This is a URL or path to the image.
        /// </summary>
        public string HeaderImage { get => Root.HeaderImage; set => Root.HeaderImage = value; }

        public List<OrigamiContentHistory> Histories { get; set; } = [];

        /// <summary>
        /// Dummy implementation to satisfy IHubContent interface, since the actual ID is stored in the Entity
        /// </summary>
        public Guid Id { get => this.Root.Id; set => this.Root.Id = value; }

        /// <summary>
        /// Necessary implementation of the INanoId interface, since the actual NanoId is stored in the Entity
        /// </summary>
        public string NanoId => this.Root.NanoId;

        public List<OrigamiContentRating> Ratings { get; set; } = [];

        public List<OrigamiContentReaction> Reactions { get; set; } = [];

        /// <summary>
        /// OrigamiContent abstraction for the main entity, root of all information here
        /// </summary>
        public abstract OrigamiContent Root { get; }

        public List<OrigamiContentTag> Tags { get; set; } = [];
    }


    public abstract class HubContent<T> :
        HubContent,
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

        public List<T> Children { get; set; } = [];

        /// <summary>
        /// The main entity, root of all information here
        /// </summary>
        public T? Entity { get; set; } = Activator.CreateInstance<T>();

        

        /// <summary>
        /// Parent element of the current object. This is used to establish a hierarchy or relationship between content items.
        /// </summary>
        public T? Parent { get; set; }

        /// <summary>
        /// Root implementation to satisfy the abstract property in the base class. This returns the main entity, which is the root of all information in this hub content.
        /// </summary>
        public override OrigamiContent Root => this.Entity;
    }
}
