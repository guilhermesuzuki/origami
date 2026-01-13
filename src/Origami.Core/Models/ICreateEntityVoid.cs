namespace Origami.Core.Models
{
    public interface ICreateEntityVoid<T>
    {
        Task CreateEntity();
    }
}
