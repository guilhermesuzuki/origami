using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISearch<T>
        where T : IId
    {
        /// <summary>
        /// This method should create the index
        /// </summary>
        /// <returns></returns>
        bool CreateSearchIndex();

        /// <summary>
        /// This method should look into the records and search with a query string
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        IEnumerable<T> Search(string searchTerm);
    }
}
