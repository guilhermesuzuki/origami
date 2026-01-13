namespace Origami.Core.Models
{
    /// <summary>
    /// TODO: rename this
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
