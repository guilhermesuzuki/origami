using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// CR[U]D operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdate<T> where T : IId
    {
        /// <summary>
        /// Updates an Entity in the database, using a <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<T> Update(DataOperationContext<T> ctx);
    }
}
