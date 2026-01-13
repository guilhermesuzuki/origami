using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_FileStoreFiles")]
    public class OrigamiFileStoreFile :
        IChanged,
        IId,
        IName,
        IDateCreated
    {
        private Guid _id = Guid.NewGuid();
        private Guid _parentDirectoryId;
        private string _name = string.Empty;
        private string _fullPath = string.Empty;
        private byte[] _contents = [];
        private int _size;
        private DateTime _dateCreated;
        private DateTime? _lastAccess;
        private DateTime? _lastModify;

        private OrigamiFileStoreDirectory? _parentDirectory;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiFileStoreFile() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        /// <summary>
        /// Parent Directory Id (FK)
        /// </summary>
        public Guid ParentDirectoryId
        {
            get { return _parentDirectoryId; }
            set { this.Set(ref _parentDirectoryId, value, Changed); }
        }

        /// <summary>
        /// Parent Directory (FK)
        /// </summary>
        [ForeignKey(nameof(ParentDirectoryId))]
        public OrigamiFileStoreDirectory? ParentDirectory
        {
            get { return _parentDirectory; }
            set { this.Set(ref _parentDirectory, value, Changed); }
        }

        [StringLength(255)]
        public string Name
        {
            get { return _name; }
            set { this.Set(ref _name, value, Changed); }
        }

        [StringLength(255)]
        public string FullPath
        {
            get { return _fullPath; }
            set { this.Set(ref _fullPath, value, Changed); }
        }

        public byte[] Contents
        {
            get { return _contents; }
            set { this.Set(ref _contents, value, Changed); }
        }

        public int Size
        {
            get { return _size; }
            set { this.Set(ref _size, value, Changed); }
        }

        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { this.Set(ref _dateCreated, value, Changed); }
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
    }
}
