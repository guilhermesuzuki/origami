using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_FileStoreFileThumbs")]
    public class OrigamiFileStoreFileThumb :
        IChanged,
        IId
    {
        private Guid _id = Guid.NewGuid();
        private Guid _fileId;
        private int _size;
        private byte[] _contents = [];

        private OrigamiFileStoreFile? _file;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiFileStoreFileThumb() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        public Guid FileId
        {
            get { return _fileId; }
            set { this.Set(ref _fileId, value, Changed); }
        }

        [ForeignKey(nameof(FileId))]
        public OrigamiFileStoreFile? File
        {
            get { return _file; }
            set { this.Set(ref _file, value, Changed); }
        }

        public int Size
        {
            get { return _size; }
            set { this.Set(ref _size, value, Changed); }
        }

        public byte[] Contents
        {
            get { return _contents; }
            set { this.Set(ref _contents, value, Changed); }
        }
    }
}
