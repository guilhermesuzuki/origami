namespace Origami.Core.Models
{
    public interface IFKPhysicalPage : IPhysicalPageId
    {
        /// <summary>
        /// Physical Page Instance for this FK relationship
        /// </summary>
        OrigamiPhysicalPage? PhysicalPage { get; set; }
    }
}
