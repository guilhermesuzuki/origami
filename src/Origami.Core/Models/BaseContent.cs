using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public abstract class BaseContent :
        BaseModel,
        IModel,
        IChanged,
        ITitle,
        IDescriptionNull,
        IContent,
        IDateCreated,
        IDateModified,
        IPublished,
        IAdditionalInfo,
        ISlug,
        IDeleted,
        IAuthorId,
        IHyperlink,
        IDraft
    {
        protected string? _additionalInfo = string.Empty;
        protected Guid _authorId;
        protected string _content = string.Empty;
        protected DateTime _dateCreated;
        protected DateTime? _dateModified;
        protected DateTime? _datePublished;
        protected string? _description = string.Empty;
        protected bool _isCommentEnabled;
        protected bool _isDeleted;
        protected bool? _isDraft;
        protected bool _isPublished;
        protected string _slug = string.Empty;
        protected string _title = string.Empty;
        protected byte[] _version = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseContent() : base()
        {
            NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
            IsDraft = true;
            IsDeleted = false;
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public virtual string? AdditionalInfo
        {
            get => _additionalInfo;
            set => this.Set(ref _additionalInfo, value, Changed);
        }

        /// <summary>
        /// Author Id (FK)
        /// </summary>
        public Guid AuthorId
        {
            get => _authorId;
            set => this.Set(ref _authorId, value, Changed);
        }

        /// <summary>
        /// Content (nvarchar[max])
        /// </summary>
        public virtual string Content
        {
            get => _content;
            set => this.Set(ref _content, value, Changed);
        }

        /// <summary>
        /// Date/Time this Content was Created
        /// </summary>
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        /// <summary>
        /// Date/Time this Page was Modified
        /// </summary>
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

        /// <summary>
        /// Description (nvarchar[max])
        /// </summary>
        public virtual string? Description
        {
            get => _description;
            set => this.Set(ref _description, value, Changed);
        }

        /// <summary>
        /// Hyperlink to this content
        /// </summary>
        public virtual string Hyperlink
        {
            get => $"/{this.GetType().GetPlural().ToLower()}/{NanoId}/";
        }

        /// <summary>
        /// Can comments be placed in this Post?
        /// </summary>
        public virtual bool IsCommentEnabled
        {
            get => _isCommentEnabled;
            set => this.Set(ref _isCommentEnabled, value, Changed);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        /// <summary>
        /// Is this content a draft?
        /// </summary>
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

        [StringLength(255)]
        public string Slug
        {
            get => _slug;
            set => this.Set(ref _slug, value, Changed);
        }

        [StringLength(255)]
        public string Title
        {
            get => _title;
            set => this.Set(ref _title, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
