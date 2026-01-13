using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_SpecialMessages")]
    public class OrigamiSpecialMessage :
        BaseModel,
        IModel,
        IChanged,
        IId,
        IType,
        ITitle,
        IDeleted,
        IPublished,
        IDateCreated,
        IDateModified,
        IContent,
        INew,
        INanoId,
        IAuthorId,
        IVersion,
        IDraft
    {
        protected Guid _authorId;
        protected string _content = string.Empty;
        protected DateTime _dateCreated = DateTime.UtcNow;
        protected DateTime? _dateModified = null;
        protected DateTime? _datePublished = null;
        protected DateOnly _endDate = DateOnly.MaxValue;
        protected bool _isDeleted = false;
        protected bool _isPublished = false;
        protected DateOnly _startDate = DateOnly.MinValue;
        protected string _title = string.Empty;
        protected string _type = OrigamiSpecialMessageTypes.None.ToString();
        protected byte[] _version = [];
        protected bool? _isDraft = true;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public OrigamiSpecialMessage()
        {
            this.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
        }

        /// <summary>
        /// Author Id (FK)
        /// </summary>
        public Guid AuthorId
        {
            get => _authorId;
            set => this.Set(ref _authorId, value, Changed);
        }

        public virtual string Content
        {
            get => _content;
            set => this.Set(ref _content, value, Changed);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        public DateTime? DatePublished
        {
            get => _datePublished;
            set => this.Set(ref _datePublished, value, Changed);
        }
        public DateOnly EndDate
        {
            get => _endDate;
            set => this.Set(ref _endDate, value, Changed);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        public bool? IsDraft
        {
            get => _isDraft;
            set => this.Set(ref _isDraft, value, Changed);
        }

        public bool IsPublished
        {
            get => _isPublished;
            set => this.Set(ref _isPublished, value, Changed);
        }

        public bool New => Version.SequenceEqual([]);

        public DateOnly StartDate
        {
            get => _startDate;
            set => this.Set(ref _startDate, value, Changed);
        }

        [StringLength(255)]
        public string Title
        {
            get => _title;
            set => this.Set(ref _title, value, Changed);
        }

        [StringLength(25)]
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }

        /// <summary>
        /// MudBlazor compatible Start Date
        /// </summary>
        [NotMapped]
        public DateTime? MB_StartDate
        {
            get => StartDate == DateOnly.MinValue ? null : StartDate.ToDateTime(new TimeOnly(0, 0));
            set => StartDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : DateOnly.MinValue;
        }

        /// <summary>
        /// MudBlazor compatible End Date
        /// </summary>
        [NotMapped]
        public DateTime? MB_EndDate
        {
            get => EndDate == DateOnly.MaxValue ? null : EndDate.ToDateTime(new TimeOnly(0, 0));
            set => EndDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : DateOnly.MaxValue;
        }
    }
}
