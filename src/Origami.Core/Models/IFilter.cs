namespace Origami.Core.Models
{
    /// <summary>
    /// Interface for filtering content based on status (e.g., all, published, draft). 
    /// The SetFilterAndRefreshUI method allows for setting the filter and then refreshing the UI to reflect the changes.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// all, published, draft
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// Sets the filter internally and refreshes the UI
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task SetFilterAndRefreshUI(string filter);
    }
}
