namespace Origami.Core.Data
{
    public interface IReadFromCache<T>
    {
        List<T> ReadFromCache();
    }
}
