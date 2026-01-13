using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Subscribers")]
    [Index(nameof(SocialProfileId), IsUnique = true, Name = "IX_oi_Subscribers_1")]
    public class OrigamiSubscriber :
        IChanged,
        IId,
        IEmail,
        IDateCreated,
        IDateModified,
        IDeleted,
        IVersion,
        IFKSocialProfile,
        INew
    {
        private DateTime _dateCreated;
        private DateTime? _dateModified;
        private string _email = string.Empty;
        private Guid _id = Guid.NewGuid();
        private bool _isDeleted;
        private bool _isVerified;
        private OrigamiSocialProfile? _socialProfile;
        private Guid _socialProfileId;
        private string? _verificationCode;
        private byte[] _version = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSubscriber() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

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

        [StringLength(100)]
        public string Email
        {
            get { return _email; }
            set { this.Set(ref _email, value, Changed); }
        }

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { this.Set(ref _isDeleted, value, Changed); }
        }

        /// <summary>
        /// Is Verified?
        /// </summary>
        public bool IsVerified
        {
            get { return _isVerified; }
            set { this.Set(ref _isVerified, value, Changed); }
        }

        public bool New => Version.SequenceEqual([]);

        [ForeignKey(nameof(SocialProfileId))]
        public OrigamiSocialProfile? SocialProfile
        {
            get { return _socialProfile; }
            set { this.Set(ref _socialProfile, value, Changed); }
        }

        public Guid SocialProfileId
        {
            get { return _socialProfileId; }
            set { this.Set(ref _socialProfileId, value, Changed); }
        }

        /// <summary>
        /// Verification Code
        /// </summary>
        [StringLength(10)]
        public string? VerificationCode
        {
            get { return _verificationCode; }
            set { this.Set(ref _verificationCode, value, Changed); }
        }

        [Timestamp]
        public byte[] Version
        {
            get { return _version; }
            set { this.Set(ref _version, value, Changed); }
        }
    }
}
