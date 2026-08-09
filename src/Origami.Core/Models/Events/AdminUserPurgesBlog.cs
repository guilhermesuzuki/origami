using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models.Events
{
    public class AdminUserPurgesBlog :
        OrigamiEvent,
        IChanged,
        IBlogId
    {
        protected Guid _blogId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public AdminUserPurgesBlog() : base()
        {
            this.Type = nameof(AdminUserPurgesBlog);
        }

        [Column(nameof(BlogId))]
        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }
    }
}
