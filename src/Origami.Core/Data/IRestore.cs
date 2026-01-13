using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IRestore<T>
        where T : IId
    {
        /// <summary>
        /// Restores the <paramref name="ctx"/> in the database
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        Result<T> Restore(DataOperationContext<T> ctx);
    }
}
