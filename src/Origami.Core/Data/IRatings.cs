using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IRatings<T, TRating> where TRating : IId
    {
        /// <summary>
        /// Calculates the rating
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        float CalculateRating(T entity);

        /// <summary>
        /// Returns all ratings from a <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IEnumerable<TRating> Ratings(T entity);

        /// <summary>
        /// Returns all ratings from a <paramref name="socialProfile"/>
        /// </summary>
        /// <param name="socialProfile"></param>
        /// <returns></returns>
        IEnumerable<TRating> RatingsFromProfile(OrigamiSocialProfile socialProfile);

        /// <summary>
        /// Creates a rating based on the <paramref name="ctx"/>, validating if it can be created
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<TRating> SmartCreate(DataOperationContextFrontEnd<TRating> ctx);

        /// <summary>
        /// Purges a rating based on the <paramref name="ctx"/>, validating if it can be purged
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<TRating> SmartPurge(DataOperationContextFrontEnd<TRating> ctx);
    }
}
