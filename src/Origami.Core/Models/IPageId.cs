namespace Origami.Core.Models
{
    public interface IPageId
    {
        /// <summary>
        /// FK from the Page Instance
        /// </summary>
        Guid PageId { get; set; }
    }
}
