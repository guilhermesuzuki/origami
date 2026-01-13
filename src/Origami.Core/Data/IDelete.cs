using Origami.Core.Models;

namespace Origami.Core.Data
{
    /// <summary>
    /// CRU[D] operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDelete<T> where T : IId
    {
        /// <summary>
        /// Removes or soft deletes the <paramref name="ctx"/>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<T> Delete(DataOperationContext<T> ctx);
    }
}
