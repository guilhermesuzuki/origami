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
        IAdditionalInfo
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
        protected Guid? _pinnedById;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseComment() : base()
        {

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

        /// <summary>
        /// Is this comment a SPAM or not?
        /// </summary>
        public bool IsSpam
        {
            get => _isSpam;
            set => this.Set(ref _isSpam, value, Changed);
        }

        /// <summary>
        /// Comment was Moderated By (ID, FK)
        /// </summary>
        public Guid? ModeratedById
        {
            get => _moderatedById;
            set => this.Set(ref _moderatedById, value, Changed);
        }

        /// <summary>
        /// Comment was Pinned By (ID, FK)
        /// </summary>
        public Guid? PinnedById
        {
            get => _pinnedById;
            set => this.Set(ref _pinnedById, value, Changed);
        }
    }
}
