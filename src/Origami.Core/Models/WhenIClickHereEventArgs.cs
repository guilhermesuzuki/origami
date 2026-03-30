using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public class WhenIClickHereEventArgs: EventArgs
    {
        /// <summary>
        /// Entity
        /// </summary>
        public IId Entity { get; }

        /// <summary>
        /// Slug for routing
        /// </summary>
        public ISlug? Slug { get; }

        /// <summary>
        /// Gets or sets a value indicating whether event propagation should be stopped after this event is handled.
        /// </summary>
        /// <remarks>Set this property to <see langword="true"/> to prevent the event from being passed to
        /// subsequent handlers. This is typically used to control event flow in event-driven architectures.</remarks>
        public bool StopPropagation { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="entity">The entity associated with the event.</param>
        public WhenIClickHereEventArgs(IId entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Default constructor with slug
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="entity"></param>
        public WhenIClickHereEventArgs(ISlug? slug, IId entity) : this(entity)
        {
            Slug = slug;
        }
    }
}
