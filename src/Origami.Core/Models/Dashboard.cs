using System.ComponentModel;

namespace Origami.Core.Models
{
    public class Dashboard :
        IId,
        IBlogId,
        IChanged
    {
        protected Guid _blogId;
        protected Guid _id;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public Guid BlogId
        {
            get { return _blogId; }
            set { this.Set(ref _blogId, value, Changed); }
        }

        public Guid Id
        {
            get => _id;
            set => _id = value;
        }
    }
}
