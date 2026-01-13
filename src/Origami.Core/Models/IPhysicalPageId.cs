namespace Origami.Core.Models
{
    public interface IPhysicalPageId
    {
        /// <summary>
        /// FK from the Physical Page Instance
        /// </summary>
        Guid PhysicalPageId { get; set; }
    }
}
