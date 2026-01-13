namespace Origami.Core.Models
{
    public interface IParentIdNull<T>
    {
        /// <summary>
        /// Parent Id (FK)
        /// </summary>
        Guid? ParentId { get; set; }
    }
}
