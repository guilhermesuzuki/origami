namespace Origami.Core.Models
{
    public abstract class HubContent<T> :
        IHubContent<T>,
        IAuthorId
        where T : OrigamiContent
    {
        protected HubContent() { }

        public Guid AuthorId
        {
            get => Entity.AuthorId;
            set => Entity.AuthorId = value;
        }

        /// <summary>
        /// The main entity, root of all information here
        /// </summary>
        public T Entity { get; set; } = Activator.CreateInstance<T>();

        public T? Parent { get; set; }

        public List<T> Children { get; set; } = [];

        public List<OrigamiContentCategory> Categories { get; set; } = [];
        public List<OrigamiContentComment> Comments { get; set; } = [];
        public List<OrigamiContentRating> Ratings { get; set; } = [];
        public List<OrigamiContentReaction> Reactions { get; set; } = [];
        public List<OrigamiContentTag> Tags { get; set; } = [];
        public List<OrigamiContentHistory> Histories { get; set; } = [];

        /// <summary>
        /// Dummy implementation to satisfy IHubContent interface, since the actual ID is stored in the Entity
        /// </summary>
        public Guid Id { get => Entity.Id; set => Entity.Id = value; }
    }
}
