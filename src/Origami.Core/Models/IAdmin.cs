namespace Origami.Core.Models
{
    public interface IAdmin
    {
        /// <summary>
        /// Is the application admin or frontend?
        /// </summary>
        bool? Admin { get; }
    }
}
