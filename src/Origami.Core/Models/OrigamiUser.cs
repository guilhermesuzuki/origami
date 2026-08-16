using NanoidDotNet;
using OtpNet;
using QRCoder;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Users")]
    public class OrigamiUser :
        BaseModel,
        IModel,
        IChanged,
        IDeleted,
        IDateCreated,
        IDateModified,
        IFirstName,
        ILastName,
        IHeaderImage,
        IHyperlink,
        IAdditionalInfo<AdditionalInfo.ForUsers>,
        IDisplayName,
        IUsername,
        IFacebook,
        IInstagram,
        IGitHub,
        IPersonalWebsite,
        ILinkedIn,
        ITOTPSecret
    {
        /// <summary>
        /// Anonymous user
        /// </summary>
        public static OrigamiUser AnonymousUser = new() { Id = Guid.Empty };

        protected string? _additionalInfo = string.Empty;
        protected DateTime? _dateBlocked;
        protected DateTime _dateCreated = DateTime.UtcNow;
        protected DateTime? _dateModified;
        protected DateTime? _dateUnblocked;
        protected string _emailAddress = string.Empty;
        protected bool _isBlocked;
        protected bool _isDeleted;
        protected DateTime? _lastLoginTime;
        protected bool _mustChangePassword;
        protected string _newPassword1 = string.Empty;
        protected string _newPassword2 = string.Empty;
        protected string _password = string.Empty;
        protected string _username = string.Empty;
        protected byte[] _version = [];

        public OrigamiUser() : base()
        {
            this._mustChangePassword = true;
            this.GenerateNewTextPasswordForNewUsers();
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public virtual string? AdditionalInfo
        {
            get => _additionalInfo;
            set => this.Set(ref _additionalInfo, value, Changed);
        }

        public DateTime? DateBlocked
        {
            get => _dateBlocked;
            set => this.Set(ref _dateBlocked, value, Changed);
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

        public DateTime? DateUnblocked
        {
            get => _dateUnblocked;
            set => this.Set(ref _dateUnblocked, value, Changed);
        }

        [NotMapped]
        public string DisplayName
        {
            get => Get().DisplayName;
            set => Set(x => x.DisplayName = value);
        }

        /// <summary>
        /// E-mail address
        /// </summary>
        [StringLength(100)]
        public string EmailAddress
        {
            get => _emailAddress;
            set => this.Set(ref _emailAddress, value, Changed);
        }

        [NotMapped]
        public string Facebook
        {
            get => Get().Facebook;
            set => Set(x => x.Facebook = value);
        }

        [NotMapped]
        public string FirstName
        {
            get => Get().FirstName;
            set => Set(x => x.FirstName = value);
        }

        [NotMapped]
        public string GitHub
        {
            get => Get().GitHub;
            set => Set(x => x.GitHub = value);
        }

        [NotMapped]
        public string HeaderImage
        {
            get => Get().HeaderImage;
            set => Set(x => x.HeaderImage = value);
        }

        public string Hyperlink => $"/users/{NanoId}/";

        [NotMapped]
        public string Instagram
        {
            get => Get().Instagram;
            set => Set(x => x.Instagram = value);
        }

        public bool IsBlocked
        {
            get => _isBlocked;
            set => this.Set(ref _isBlocked, value, Changed);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        /// <summary>
        /// Last Time the User logged in
        /// </summary>
        public DateTime? LastLoginTime
        {
            get => _lastLoginTime;
            set => this.Set(ref _lastLoginTime, value, Changed);
        }

        [NotMapped]
        public string LastName
        {
            get => Get().LastName;
            set => Set(x => x.LastName = value);
        }

        [NotMapped]
        public string LinkedIn
        {
            get => Get().LinkedIn;
            set => Set(x => x.LinkedIn = value);
        }

        public bool MustChangePassword
        {
            get => _mustChangePassword;
            set => this.Set(ref _mustChangePassword, value, Changed);
        }

        public bool New => Version.SequenceEqual([]);

        [NotMapped]
        public string NewPassword1
        {
            get => _newPassword1;
            set => this.Set(ref _newPassword1, value, Changed);
        }

        [NotMapped]
        public string NewPassword2
        {
            get => _newPassword2;
            set => this.Set(ref _newPassword2, value, Changed);
        }

        /// <summary>
        /// New Password
        /// </summary>
        [StringLength(1000)]
        public string Password
        {
            get => _password;
            set => this.Set(ref _password, value, Changed);
        }

        [NotMapped]
        public string TOTPRecoveryCodes
        {
            get => Get().TOTPRecoveryCodes;
            set => Set(x => x.TOTPRecoveryCodes = value);
        }

        [NotMapped]
        public string TOTPSecret
        {
            get => Get().TOTPSecret;
            set => Set(x => x.TOTPSecret = value);
        }

        [NotMapped]
        public List<OrigamiUserBlog> UserBlogs { get; set; } = new();

        /// <summary>
        /// Username
        /// </summary>
        [StringLength(100)]
        public string Username
        {
            get => _username;
            set => this.Set(ref _username, value, Changed);
        }

        [NotMapped]
        public List<OrigamiUserRole> UserRoles { get; set; } = new();

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }

        [NotMapped]
        public string Website
        {
            get => Get().Website;
            set => Set(x => x.Website = value);
        }

        public bool ConsumeTOTPRecoveryCode(string totpCodeForValidation)
        {
            var hash = totpCodeForValidation.SHA256Hash();
            var recoveryCodes = GetTOTPRecoveryCodes();
            var found = recoveryCodes.FirstOrDefault(x => x == hash);
            if (found != null)
            {
                recoveryCodes = recoveryCodes.Where(x => x != hash).ToArray();
                TOTPRecoveryCodes = string.Join(",", recoveryCodes);
                return true;
            }
            return false;
        }

        public void GenerateNewTextPasswordForNewUsers()
        {
            this._newPassword1 = Nanoid.Generate("!@#$%&*", size: 1) + Nanoid.Generate(size: 9);
            this._newPassword2 = this._newPassword1;
        }

        public void GenerateRandomTOTPSecret()
        {
            // Generate 20 bytes (160-bit secret)
            var secretBytes = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretBytes);
            TOTPSecret = base32Secret;
        }

        public AdditionalInfo.ForUsers Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForUsers>();
        }

        public string GetTOTPQrCode()
        {
            var uri = this.GetTOTPUri();
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrData);
            string qrImage = qrCode.GetGraphic(20);
            return qrImage;
        }

        public string[] GetTOTPRecoveryCodes()
        {
            return TOTPRecoveryCodes.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public string GetTOTPUri()
        {
            var app = "Origami";
            return $"otpauth://totp/{app}:{Username}?secret={TOTPSecret}&issuer={app}";
        }

        public AdditionalInfo.ForUsers Set(Action<AdditionalInfo.ForUsers> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForUsers>();
        }
    }
}
