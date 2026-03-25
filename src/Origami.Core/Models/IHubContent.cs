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
    }
}
