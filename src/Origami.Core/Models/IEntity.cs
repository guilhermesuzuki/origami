namespace Origami.Core.Models
{
    public interface IEntity<T>
    {
        T Entity { get; set; }
    }
}
