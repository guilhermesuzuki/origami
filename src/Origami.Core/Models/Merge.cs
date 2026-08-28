namespace Origami.Core.Models
{
    public class Merge<T>
    {
        public Merge() : base()
        {

        }

        public Merge(IEnumerable<T> purge, IEnumerable<T> update, IEnumerable<T> create) : this()
        {
            Purge = purge;
            Update = update;
            Create = create;
        }

        public IEnumerable<T> Create { get; set; } = Array.Empty<T>();
        public IEnumerable<T> Purge { get; set; } = Array.Empty<T>();
        public IEnumerable<T> Update { get; set; } = Array.Empty<T>();
    }
}
