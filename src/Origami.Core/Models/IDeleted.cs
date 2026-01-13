namespace Origami.Core.Models
{
    public interface IDeleted
    {
        /// <summary>
        /// Is this Entity Deleted or not?
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
