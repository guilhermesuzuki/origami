using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// [C]RUD operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICreate<T> where T : IId
    {
        /// <summary>
        /// Creates the entity in the database
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Result of the Operation (ok or not)</returns>
        Result<T> Create(DataOperationContext<T> ctx);
    }
}
