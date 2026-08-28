using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models.Events
{
    public class AdminUserDeletesBlog :
        OrigamiEvent,
        IChanged,
        IBlogId
    {
        protected Guid _blogId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public AdminUserDeletesBlog() : base()
        {
            this.Type = nameof(AdminUserDeletesBlog);
        }

        [Column(nameof(BlogId))]
        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }
    }
}
