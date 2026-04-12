namespace Origami.Core.Models
{
    public interface IFKParentNull<T> : IParentIdNull
    {
        /// <summary>
        /// Parent (FK)
        /// </summary>
        T? Parent { get; set; }
    }
}
