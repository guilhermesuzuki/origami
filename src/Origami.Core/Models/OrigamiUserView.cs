using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public class OrigamiUserView :
        BaseView,
        IViewChanged,
        IId,
        IBlogId,
        IAdmin
    {
        protected bool? _admin;
        protected Guid _blogId = Guid.Empty;
        protected Guid _id = Guid.NewGuid();
        protected string _type = string.Empty;
        protected string _typeName = string.Empty;
        protected Guid? _userId;
        public event EventHandler<PropertyChangedEventArgs> ViewChanged = (sender, e) => { };

        public bool? Admin
        {
            get => _admin;
            set => this.Set(ref _admin, value, ViewChanged);
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, ViewChanged);
        }

        [Key]
        public Guid FakeId { get; set; }

        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ViewChanged);
        }

        /// <summary>
        /// System Type
        /// </summary>
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, ViewChanged);
        }

        /// <summary>
        /// Type, but human readable
        /// </summary>
        public string TypeName
        {
            get => _typeName;
            set => this.Set(ref _typeName, value, ViewChanged);
        }

        public Guid? UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, ViewChanged);
        }
    }
}
