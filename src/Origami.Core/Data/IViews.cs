namespace Origami.Core.Data
{
    public interface IViews<T>
    {
        /// <summary>
        /// Returns the total number of views from a <typeparamref name="T"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        long GetViews(T entity);

        /// <summary>
        /// Sets the total number of views from a <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="count">total view count</param>
        /// <returns></returns>
        void SetViews(T entity, long count);
    }
}
