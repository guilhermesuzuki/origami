using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public abstract class BaseComment :
        BaseTracking,
        IId,
        IChanged,
        IContent,
        IDateModified,
        IDeleted,
        IAdditionalInfo,
        INanoId
    {
        protected string? _additionalInfo;
        protected string _content = string.Empty;
        protected DateTime? _dateModified;
        protected Guid _id = Guid.NewGuid();
        protected string _ip = string.Empty;
        protected bool _isApproved;
        protected bool _isDeleted;
        protected bool _isSpam;
        protected Guid? _moderatedById;
        protected Guid? _moderatedByUserId;
        protected string _nanoId;
        protected Guid? _pinnedById;

        protected Guid? _pinnedByUserId;
        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseComment() : base()
        {
            _nanoId = NanoidDotNet.Nanoid.Generate(NanoidDotNet.Nanoid.Alphabets.LettersAndDigits, size: 8);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public string? AdditionalInfo
        {
            get => _additionalInfo;
            set => this.Set(ref _additionalInfo, value, Changed);
        }

        public string Content
        {
            get => _content;
            set => this.Set(ref _content, value, Changed);
        }

        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// IP Address
        /// </summary>
        [StringLength(50)]
        public string Ip
        {
            get => _ip;
            set => this.Set(ref _ip, value, Changed);
        }

        /// <summary>
        /// Is this comment approved or not?
        /// </summary>
        public bool IsApproved
        {
            get => _isApproved;
            set => this.Set(ref _isApproved, value, Changed);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        public bool IsModeratedBySomeone => ModeratedById != null || ModeratedByUserId != null;

        public bool IsPinnedBySomeone => PinnedById != null || PinnedByUserId != null;

        /// <summary>
        /// Is this comment a SPAM or not?
        /// </summary>
        public bool IsSpam
        {
            get => _isSpam;
            set => this.Set(ref _isSpam, value, Changed);
        }
        /// <summary>
        /// Comment was moderated by a social profile (ID, FK)
        /// </summary>
        public Guid? ModeratedById
        {
            get => _moderatedById;
            set => this.Set(ref _moderatedById, value, Changed);
        }

        /// <summary>
        /// Comment was moderated by an admin user (ID, FK)
        /// </summary>
        public Guid? ModeratedByUserId
        {
            get => _moderatedByUserId;
            set => this.Set(ref _moderatedByUserId, value, Changed);
        }

        [StringLength(8)]
        public string NanoId
        {
            get => _nanoId;
            set => this.Set(ref _nanoId, value, Changed);
        }

        /// <summary>
        /// Comment was pinned by a social profile (ID, FK)
        /// </summary>
        public Guid? PinnedById
        {
            get => _pinnedById;
            set => this.Set(ref _pinnedById, value, Changed);
        }

        /// <summary>
        /// Comment was pinned by an admin user (ID, FK)
        /// </summary>
        public Guid? PinnedByUserId
        {
            get => _pinnedByUserId;
            set => this.Set(ref _pinnedByUserId, value, Changed);
        }
    }
}
