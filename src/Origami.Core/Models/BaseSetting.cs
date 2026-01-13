using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public abstract class BaseSetting :
        IChanged,
        IBlogId,
        IName,
        IUsername
    {
        protected Guid _blogId;
        protected string _name = string.Empty;
        protected string _userName = string.Empty;
        protected string _value = string.Empty;

        public BaseSetting() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid BlogId
        {
            get { return _blogId; }
            set { this.Set(ref _blogId, value, Changed); }
        }

        [StringLength(200)]
        public virtual string Name
        {
            get { return _name; }
            set { this.Set(ref _name, value, Changed); }
        }

        [StringLength(100)]
        public virtual string Username
        {
            get { return _userName; }
            set { this.Set(ref _userName, value, Changed); }
        }
        public virtual string Value
        {
            get { return _value; }
            set { this.Set(ref _value, value, Changed); }
        }
    }
}
