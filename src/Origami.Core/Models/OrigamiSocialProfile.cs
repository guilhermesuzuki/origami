using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_SocialProfiles")]
    public class OrigamiSocialProfile :
        BaseModel,
        IChanged,
        IName,
        IEmail,
        IVersion,
        IAdditionalInfo,
        INew,
        IHyperlink
    {
        private string? _additionalInfo = string.Empty;
        private string _email = string.Empty;
        private string _emailFromSocialNetwork = string.Empty;
        private string _firstName = string.Empty;
        private bool _isBlocked;
        private bool _isModerator;
        private string _lastName = string.Empty;
        private string _name = string.Empty;
        private string? _profileCover;
        private string? _profileCoverUrl;
        private string? _profilePage;
        private string? _profilePicture;
        private string? _profilePictureUrl;
        private SocialNetworks _socialProfile;
        private string _userId = string.Empty;
        private byte[] _version = Array.Empty<byte>();

        public OrigamiSocialProfile() : base()
        {

        }

        /// <summary>
        /// Anonymous user
        /// </summary>
        public static OrigamiSocialProfile AnonymousUser => new() { Id = Guid.Empty };

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public string? AdditionalInfo
        {
            get => _additionalInfo;
            set => this.Set(ref _additionalInfo, value, Changed);
        }

        /// <summary>
        /// Email (to be used by the Application)
        /// </summary>
        [StringLength(255)]
        public string Email
        {
            get => _email;
            set => this.Set(ref _email, value, Changed);
        }

        /// <summary>
        /// Email (saved from the Social Network)
        /// </summary>
        [StringLength(255)]
        public string EmailFromSocialNetwork
        {
            get => _emailFromSocialNetwork;
            set => this.Set(ref _emailFromSocialNetwork, value, Changed);
        }

        /// <summary>
        /// First name
        /// </summary>
        [StringLength(50)]
        public string FirstName
        {
            get => _firstName;
            set => this.Set(ref _firstName, value, Changed);
        }

        /// <summary>
        /// Is this Social Profile Blocked?
        /// </summary>
        public bool IsBlocked
        {
            get => _isBlocked;
            set => this.Set(ref _isBlocked, value, Changed);
        }

        /// <summary>
        /// Is this Social Profile a Moderator?
        /// </summary>
        public bool IsModerator
        {
            get => _isModerator;
            set => this.Set(ref _isModerator, value, Changed);
        }

        /// <summary>
        /// Last name
        /// </summary>
        [StringLength(150)]
        public string LastName
        {
            get => _lastName;
            set => this.Set(ref _lastName, value, Changed);
        }

        /// <summary>
        /// Name
        /// </summary>
        [StringLength(200)]
        public string Name
        {
            get => _name;
            set => this.Set(ref _name, value, Changed);
        }

        public bool New => Version.SequenceEqual([]);

        /// <summary>
        /// Profile Cover (Base64)
        /// </summary>
        public string? ProfileCover
        {
            get => _profileCover;
            set => this.Set(ref _profileCover, value, Changed);
        }

        /// <summary>
        /// Profile Cover (URL)
        /// </summary>
        [StringLength(2048)]
        public string? ProfileCoverUrl
        {
            get => _profileCoverUrl;
            set => this.Set(ref _profileCoverUrl, value, Changed);
        }

        /// <summary>
        /// Profile Page (URL)
        /// </summary>
        [StringLength(2048)]
        public string? ProfilePage
        {
            get => _profilePage;
            set => this.Set(ref _profilePage, value, Changed);
        }

        /// <summary>
        /// Profile Picture (Base64)
        /// </summary>
        public string? ProfilePicture
        {
            get => _profilePicture;
            set => this.Set(ref _profilePicture, value, Changed);
        }

        /// <summary>
        /// Profile Picture (URL)
        /// </summary>
        [StringLength(2048)]
        public string? ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set => this.Set(ref _profilePictureUrl, value, Changed);
        }

        /// <summary>
        /// Social Network
        /// </summary>
        [StringLength(25)]
        public SocialNetworks SocialNetwork
        {
            get => _socialProfile;
            set => this.Set(ref _socialProfile, value, Changed);
        }

        /// <summary>
        /// User Id from the Social Network
        /// </summary>
        [StringLength(50)]
        public string UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }

        public string Hyperlink => $"/socialprofiles/{this.Id}";

        /// <summary>
        /// First, it tries to get the e-mail from the subscription.
        /// Then, it queries the application e-mail or the e-mail coming from the social network (when shared).
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            if (Email.Has() == true) return Email;
            if (EmailFromSocialNetwork.Has() == true) return EmailFromSocialNetwork;
            return "•• Not shared ••";
        }

        /// <summary>
        /// Does this Social Profile has an e-mail address?
        /// </summary>
        /// <returns></returns>
        public bool HasEmail()
        {
            if (Email.Has() == true) return true;
            if (EmailFromSocialNetwork.Has() == true) return true;
            return false;
        }
    }
}
