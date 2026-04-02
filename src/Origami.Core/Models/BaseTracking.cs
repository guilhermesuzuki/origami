using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public abstract class BaseTracking :
        IDateCreated,
        IVersion,
        ISocialProfileIdNull,
        IUserIdNull,
        ILocation,
        INew
    {
        protected string _browser = string.Empty;
        protected DateTime _dateCreated = DateTime.UtcNow;
        protected string _hostAddress = string.Empty;
        protected string _hostName = string.Empty;
        protected bool _isBot;
        protected bool _isMobileDevice;
        protected Location? _location;
        protected string _platform = string.Empty;
        protected Guid? _socialProfileId;
        protected string? _url = string.Empty;
        protected string? _urlReferrer = string.Empty;
        protected string _userAgent = string.Empty;
        protected Guid? _userId;
        protected byte[] _version = [];

        protected BaseTracking() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> TrackingChanged = (sender, e) => { };

        /// <summary>
        /// Browser
        /// </summary>
        [StringLength(100)]
        public string Browser
        {
            get => _browser;
            set => this.Set(ref _browser, value, TrackingChanged);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, TrackingChanged);
        }

        /// <summary>
        /// Host Address (IP)
        /// </summary>
        [StringLength(100)]
        public string HostAddress
        {
            get => _hostAddress;
            set => this.Set(ref _hostAddress, value, TrackingChanged);
        }

        /// <summary>
        /// Host Name
        /// </summary>
        [StringLength(100)]
        public string HostName
        {
            get => _hostName;
            set => this.Set(ref _hostName, value, TrackingChanged);
        }

        /// <summary>
        /// Is it a bot?
        /// </summary>
        public bool IsBot
        {
            get => _isBot;
            set => this.Set(ref _isBot, value, TrackingChanged);
        }

        /// <summary>
        /// Is it a Mobile Device?
        /// </summary>
        public bool IsMobileDevice
        {
            get => _isMobileDevice;
            set => this.Set(ref _isMobileDevice, value, TrackingChanged);
        }

        /// <summary>
        /// Location associated with this View
        /// </summary>
        public Location? Location
        {
            get => _location;
            set => this.Set(ref _location, value, TrackingChanged);
        }

        public bool New => this.Version.SequenceEqual(Array.Empty<byte>());

        /// <summary>
        /// Platform
        /// </summary>
        [StringLength(100)]
        public string Platform
        {
            get => _platform;
            set => this.Set(ref _platform, value, TrackingChanged);
        }

        /// <summary>
        /// Social Profile associated with this information, when available (e.g., for authenticated social profiles)
        /// </summary>
        public Guid? SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, TrackingChanged);
        }

        /// <summary>
        /// URL
        /// </summary>
        [StringLength(2048)]
        public string? Url
        {
            get => _url;
            set => this.Set(ref _url, value, TrackingChanged);
        }

        /// <summary>
        /// URL Referrer
        /// </summary>
        [StringLength(2048)]
        public string? UrlReferrer
        {
            get => _urlReferrer;
            set => this.Set(ref _urlReferrer, value, TrackingChanged);
        }

        /// <summary>
        /// User Agent
        /// </summary>
        public string UserAgent
        {
            get => _userAgent;
            set => this.Set(ref _userAgent, value, TrackingChanged);
        }

        /// <summary>
        /// User associated with this information, when available (e.g., for authenticated users)
        /// </summary>
        public Guid? UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, TrackingChanged);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, TrackingChanged);
        }
    }
}
