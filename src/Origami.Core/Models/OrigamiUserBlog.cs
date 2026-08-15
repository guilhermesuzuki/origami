using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_UserBlogs")]
    public class OrigamiUserBlog : BaseModel,
        IChanged,
        IBlogId,
        IEnabled
    {
        protected bool _enabled = true;
        protected Guid _blogId;
        protected Guid _userId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [NotMapped]
        public bool Enabled
        {
            get { return _enabled; }
            set { this.Set(ref _enabled, value, Changed); }
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        public Guid UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, Changed);
        }
    }
}
