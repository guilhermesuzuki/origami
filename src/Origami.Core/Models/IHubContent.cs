using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    /// <summary>
    /// Hub interface for content entities, which includes categories and tags.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHubContent<T> : IHub<T> where T : OrigamiContent
    {
        /// <summary>
        /// Categories associated with the entity
        /// </summary>
        List<OrigamiContentCategory> Categories { get; }
        
        /// <summary>
        /// Comments associated with the entity
        /// </summary>
        List<OrigamiContentComment> Comments { get; }
        
        /// <summary>
        /// Ratings associated with the entity
        /// </summary>
        List<OrigamiContentRating> Ratings { get; }
        
        /// <summary>
        /// Reactions associated with the entity
        /// </summary>
        List<OrigamiContentReaction> Reactions { get; }

        /// <summary>
        /// Tags associated with the entity
        /// </summary>
        List<OrigamiContentTag> Tags { get; }
    }
}
