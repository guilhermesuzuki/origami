namespace Origami.Core.Models
{
    public interface IItems<T>
    {
        List<T> Items { get; set; }
    }
}
