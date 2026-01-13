namespace Origami.Core.Models
{
    public class EntityOperation<T> :
        IEntity<T>
    {
        public EntityOperation(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; set; }

        /// <summary>
        /// Indicates that the <see cref="Entity"/> has been added.
        /// </summary>
        public bool EntityHasBeenAdded { get; init; }

        /// <summary>
        /// Indicates that the <see cref="Entity"/> has been deleted.
        /// </summary>
        public bool EntityHasBeenDeleted { get; init; }

        /// <summary>
        /// Indicates that the <see cref="Entity"/> has been updated.
        /// </summary>
        public bool EntityHasBeenUpdated { get; init; }
    }

    public class EntityOperation : EntityOperation<IId>
    {
        public EntityOperation(IId entity) : base(entity)
        {

        }
    }
}
