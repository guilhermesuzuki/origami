namespace Origami.Core.Models
{
    public interface IParentIdNull
    {
        /// <summary>
        /// Parent Id (FK)
        /// </summary>
        Guid? ParentId { get; set; }
    }
}
