namespace Origami.Core.Models
{
    /// <summary>
    /// Simple data operation context
    /// </summary>
    public class DataOperationContext
    {
        public DataOperationContext(OrigamiUser user, DateTime dateTime)
        {
            User = user;
            DateTime = dateTime;
        }

        /// <summary>
        /// Date and time it happened
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// User who performed the operation
        /// </summary>
        public OrigamiUser User { get; }
    }

    /// <summary>
    /// Data operation context with <see cref="Entity"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataOperationContext<T> : DataOperationContext
    {
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entity"></param>
        public DataOperationContext(OrigamiUser user, T entity) : base(user, DateTime.UtcNow)
        {
            Entity = entity;
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dateTime"></param>
        /// <param name="entity"></param>
        public DataOperationContext(OrigamiUser user, DateTime dateTime, T entity) : base(user, dateTime)
        {
            Entity = entity;
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entity"></param>
        public DataOperationContext(OrigamiUser user, T entity, T entityBefore) : base(user, DateTime.UtcNow)
        {
            Entity = entity;
            EntityBeforeModifications = entityBefore;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dateTime"></param>
        /// <param name="entity"></param>
        public DataOperationContext(OrigamiUser user, DateTime dateTime, T entity, T? entityBefore) : base(user, dateTime)
        {
            Entity = entity;
            EntityBeforeModifications = entityBefore;
        }

        public T Entity { get; }

        public T? EntityBeforeModifications { get; }
    }
}
