using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_BackupRestores")]
    public class OrigamiBackup : BaseModel,
        IModel,
        IChanged,
        IDateCreated,
        IDateModified,
        IProgress,
        IAuthorId
    {
        protected Guid _authorId = Guid.Empty;
        protected DateTime _dateCreated = DateTime.UtcNow;
        protected DateTime? _dateModified;
        protected byte _progress = 0;
        protected byte[] _version = Array.Empty<byte>();

        public OrigamiBackup() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = delegate { };

        public Guid AuthorId
        {
            get => _authorId;
            set => this.Set(ref _authorId, value, Changed);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        /// <summary>
        /// Date and time the backup/restore was last modified
        /// </summary>
        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        /// <summary>
        /// Gets the relative file path of the backup archive associated with this instance.
        /// </summary>
        public string File => $"/files-backup/{Filename}";

        /// <summary>
        /// Gets only the filename of the backup
        /// </summary>
        public string Filename => $"origami-backup-{NanoId}.zip";

        public bool New => this.Version.SequenceEqual([]);

        /// <summary>
        /// The progress of the backup (0-100)
        /// </summary>
        public byte Progress
        {
            get => _progress;
            set => this.Set(ref _progress, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
