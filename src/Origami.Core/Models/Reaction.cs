using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    public abstract class Reaction :
        IChanged,
        IDateCreated,
        IVersion,
        IFKSocialProfile,
        ILocation
    {
        protected string _browser = string.Empty;
        protected DateTime _dateCreated = DateTime.UtcNow;
        protected string _hostAddress = string.Empty;
        protected string _hostName = string.Empty;
        protected bool _isMobileDevice;
        protected Location? _location;
        protected string _platform = string.Empty;
        protected OrigamiSocialProfile? _socialProfile;
        protected Guid _socialProfileId;
        protected string? _url = string.Empty;
        protected string? _urlReferrer = string.Empty;
        protected string _userAgent = string.Empty;
        protected byte[] _version = [];

        public Reaction() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Browser
        /// </summary>
        [StringLength(100)]
        public string Browser
        {
            get => _browser;
            set => this.Set(ref _browser, value, Changed);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        /// <summary>
        /// Host Address (IP)
        /// </summary>
        [StringLength(100)]
        public string HostAddress
        {
            get => _hostAddress;
            set => this.Set(ref _hostAddress, value, Changed);
        }

        /// <summary>
        /// Host Name
        /// </summary>
        [StringLength(100)]
        public string HostName
        {
            get => _hostName;
            set => this.Set(ref _hostName, value, Changed);
        }

        /// <summary>
        /// Is it a Mobile Device?
        /// </summary>
        public bool IsMobileDevice
        {
            get => _isMobileDevice;
            set => this.Set(ref _isMobileDevice, value, Changed);
        }

        /// <summary>
        /// Location associated with this View
        /// </summary>
        public Location? Location
        {
            get => _location;
            set => this.Set(ref _location, value, Changed);
        }

        /// <summary>
        /// Platform
        /// </summary>
        [StringLength(100)]
        public string Platform
        {
            get => _platform;
            set => this.Set(ref _platform, value, Changed);
        }

        [ForeignKey(nameof(SocialProfileId))]
        public OrigamiSocialProfile? SocialProfile
        {
            get => _socialProfile;
            set => this.Set(ref _socialProfile, value, Changed);
        }

        public Guid SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, Changed);
        }

        /// <summary>
        /// URL
        /// </summary>
        [StringLength(2048)]
        public string? Url
        {
            get => _url;
            set => this.Set(ref _url, value, Changed);
        }

        /// <summary>
        /// URL Referrer
        /// </summary>
        [StringLength(2048)]
        public string? UrlReferrer
        {
            get => _urlReferrer;
            set => this.Set(ref _urlReferrer, value, Changed);
        }

        /// <summary>
        /// User Agent
        /// </summary>
        public string UserAgent
        {
            get => _userAgent;
            set => this.Set(ref _userAgent, value, Changed);
        }
        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
