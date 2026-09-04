namespace Origami.Core.Data
{
    public interface ITheCreator
    {
        T Create<T>() where T : class, new();
    }
}
