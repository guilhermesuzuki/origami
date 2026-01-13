using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IPurge<T> where T : IId
    {
        /// <summary>
        /// Removes the <paramref name="ctx"/> permanently in the database
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<T> Purge(DataOperationContext<T> ctx);
    }
}
