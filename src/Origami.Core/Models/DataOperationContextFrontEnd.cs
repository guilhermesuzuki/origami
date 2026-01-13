namespace Origami.Core.Models
{
    public class DataOperationContextFrontEnd<T> : DataOperationContext<T>
    {
        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="socialProfile">social profile responsible for this operation</param>
        /// <param name="dateTime">date and time (when the operation occurred)</param>
        /// <param name="entity">the entity with the modifications to be persisted</param>
        public DataOperationContextFrontEnd(OrigamiSocialProfile socialProfile, DateTime dateTime, T entity) : base(OrigamiUser.AnonymousUser, dateTime, entity)
        {
            SocialProfile = socialProfile;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="socialProfile">social profile responsible for this operation</param>
        /// <param name="dateTime">date and time (when the operation occurred)</param>
        /// <param name="entity">the entity with the modifications to be persisted</param>
        /// <param name="entityBefore">the entity before the modifications</param>
        public DataOperationContextFrontEnd(OrigamiSocialProfile socialProfile, DateTime dateTime, T entity, T entityBefore) : base(OrigamiUser.AnonymousUser, dateTime, entity, entityBefore)
        {
            SocialProfile = socialProfile;
        }

        /// <summary>
        /// Social profile who performed the operation
        /// </summary>
        public OrigamiSocialProfile SocialProfile { get; }
    }
}
