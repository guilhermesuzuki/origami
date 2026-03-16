using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_QuickNotes")]
    public class OrigamiQuickNote : BaseModel,
        IChanged,
        IId,
        IBlogId,
        IAuthorId,
        IDateCreated,
        IDateModified,
        IDeleted,
        ILanguageWrittenOn,
        IVersion,
        INew
    {
        private Guid _authorId;
        private Guid _blogId;
        private DateTime _dateCreated;
        private DateTime? _dateModified;
        private bool _isDeleted = false;
        private string _languageWrittenOn = string.Empty;
        private string _note = string.Empty;
        private byte[] _version = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiQuickNote() : base()
        {
            this.LanguageWrittenOn = Thread.CurrentThread.CurrentUICulture.Name;
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid AuthorId
        {
            get { return _authorId; }
            set { this.Set(ref _authorId, value, Changed); }
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

        public DateTime? DateModified
        {
            get { return _dateModified; }
            set { this.Set(ref _dateModified, value, Changed); }
        }

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { this.Set(ref _isDeleted, value, Changed); }
        }

        [StringLength(5)]
        public string LanguageWrittenOn
        {
            get { return _languageWrittenOn; }
            set { this.Set(ref _languageWrittenOn, value, Changed); }
        }

        public bool New => _version.SequenceEqual([]);

        [StringLength(256)]
        public string Note
        {
            get { return _note; }
            set { this.Set(ref _note, value, Changed); }
        }

        [Timestamp]
        public byte[] Version
        {
            get { return _version; }
            set { this.Set(ref _version, value, Changed); }
        }
    }
}
