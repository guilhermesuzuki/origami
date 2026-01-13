using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_QuickNotes")]
    public class OrigamiQuickNote :
        IChanged,
        IId,
        IFKBlog
    {
        private Guid _id = Guid.NewGuid();
        private Guid _blogId;
        private string _userName = string.Empty;
        private string _note = string.Empty;
        private DateTime? _updated;

        private OrigamiBlog? _blog;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiQuickNote() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        public Guid BlogId
        {
            get { return _blogId; }
            set { this.Set(ref _blogId, value, Changed); }
        }

        [ForeignKey(nameof(BlogId))]
        public OrigamiBlog? Blog
        {
            get { return _blog; }
            set { this.Set(ref _blog, value, Changed); }
        }

        [StringLength(100)]
        public string UserName
        {
            get { return _userName; }
            set { this.Set(ref _userName, value, Changed); }
        }

        public string Note
        {
            get { return _note; }
            set { this.Set(ref _note, value, Changed); }
        }

        public DateTime? Updated
        {
            get { return _updated; }
            set { this.Set(ref _updated, value, Changed); }
        }
    }
}
