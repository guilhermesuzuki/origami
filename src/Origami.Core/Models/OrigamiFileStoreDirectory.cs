using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_FileStoreDirectories")]
    public class OrigamiFileStoreDirectory :
        IChanged,
        IId,
        IFKBlog,
        IFKParentNull<OrigamiFileStoreDirectory>,
        IDateCreated,
        IName
    {
        private OrigamiBlog? _blog;
        private Guid _blogId;
        private DateTime _dateCreated;
        private string _fullPath = string.Empty;
        private Guid _id = Guid.NewGuid();
        private DateTime? _lastAccess;
        private DateTime? _lastModify;
        private string _name = string.Empty;
        private OrigamiFileStoreDirectory? _parent;
        private Guid? _parentId;
        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiFileStoreDirectory() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [ForeignKey(nameof(BlogId))]
        public OrigamiBlog? Blog
        {
            get { return _blog; }
            set { this.Set(ref _blog, value, Changed); }
        }

        public Guid BlogId
        {
            get { return _blogId; }
            set { this.Set(ref _blogId, value, Changed); }
        }

        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { this.Set(ref _dateCreated, value, Changed); }
        }

        [StringLength(1000)]
        public string FullPath
        {
            get { return _fullPath; }
            set { this.Set(ref _fullPath, value, Changed); }
        }

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        public DateTime? LastAccess
        {
            get { return _lastAccess; }
            set { this.Set(ref _lastAccess, value, Changed); }
        }

        public DateTime? LastModify
        {
            get { return _lastModify; }
            set { this.Set(ref _lastModify, value, Changed); }
        }

        [StringLength(255)]
        public string Name
        {
            get { return _name; }
            set { this.Set(ref _name, value, Changed); }
        }

        [ForeignKey(nameof(ParentId))]
        public OrigamiFileStoreDirectory? Parent
        {
            get { return _parent; }
            set { this.Set(ref _parent, value, Changed); }
        }

        public Guid? ParentId
        {
            get { return _parentId; }
            set { this.Set(ref _parentId, value, Changed); }
        }
    }
}
