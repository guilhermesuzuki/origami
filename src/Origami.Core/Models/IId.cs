namespace Origami.Core.Models
{
    public interface IId
    {
        /// <summary>
        /// System Id (Primary Key)
        /// </summary>
        Guid Id { get; set; }
    }
}
