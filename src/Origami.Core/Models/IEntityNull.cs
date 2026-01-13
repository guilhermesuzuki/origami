namespace Origami.Core.Models
{
    public interface IEntityNull<T>
    {
        T? Entity { get; set; }
    }
}
