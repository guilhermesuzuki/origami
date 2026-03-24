using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public abstract class HubContent<T> : IHubContent<T> where T : OrigamiContent
    {
        protected HubContent() { }

        /// <summary>
        /// The main entity, root of all information here
        /// </summary>
        public T Entity { get; set; } = Activator.CreateInstance<T>();

        public List<OrigamiContentCategory> Categories { get; } = [];

        public List<OrigamiContentComment> Comments { get; } = [];

        public List<OrigamiContentRating> Ratings { get; } = [];

        public List<OrigamiContentReaction> Reactions { get; } = [];

        public List<OrigamiContentTag> Tags { get; } = [];

        /// <summary>
        /// Dummy implementation to satisfy IHubContent interface, since the actual ID is stored in the Entity
        /// </summary>
        public Guid Id { get => Entity.Id; set => Entity.Id = value; }
    }
}
