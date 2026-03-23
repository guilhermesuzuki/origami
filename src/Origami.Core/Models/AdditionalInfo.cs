using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Origami.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot("additionalInfo")]
    public class AdditionalInfo : IDisposable, IHeaderImage
    {
        /// <summary>
        /// constructor
        /// </summary>
        public AdditionalInfo()
        {
            Localizations = new List<Localization>();
        }

        /// <summary>
        /// client information
        /// </summary>
        public interface IClientTracking
        {
            /// <summary>
            /// Is the browser a crawler?
            /// </summary>
            bool Crawler { get; set; }

            /// <summary>
            /// date and time the event occurred (yyyy-MM-dd HH:mm)
            /// </summary>
            string DateTime { get; set; }

            /// <summary>
            /// client host address
            /// </summary>
            string HostAddress { get; set; }

            /// <summary>
            /// client host name
            /// </summary>
            string HostName { get; set; }

            /// <summary>
            /// Is this tracking information related to a Bot?
            /// </summary>
            bool IsBot { get; }

            /// <summary>
            /// Location of this IP
            /// </summary>
            Location Location { get; set; }

            /// <summary>
            /// Information about the current URL.
            /// </summary>
            string Url { get; set; }

            /// <summary>
            /// Client request that links to the current URL.
            /// </summary>
            string UrlReferrer { get; set; }

            /// <summary>
            /// client useragent
            /// </summary>
            string UserAgent { get; set; }

            /// <summary>
            /// useragent (100 characters)
            /// </summary>
            string UserAgentAbbreviated { get; }

            /// <summary>
            /// User Profile Id (originally Guid, changed to string to save xml caracters)
            /// </summary>
            string UserProfile { get; set; }
        }

        /// <summary>
        /// Interface for Location
        /// </summary>
        public interface ILocation
        {
            /// <summary>
            /// name of the city
            /// </summary>
            string City { get; set; }

            /// <summary>
            /// name of the country
            /// </summary>
            string Country { get; set; }

            /// <summary>
            /// Country code
            /// </summary>
            string CountryCode { get; set; }

            /// <summary>
            /// IP Address
            /// </summary>
            string Ip { get; set; }

            /// <summary>
            /// latitude of the location
            /// </summary>
            float? Latitude { get; set; }

            /// <summary>
            /// longitude of the location
            /// </summary>
            float? Longitude { get; set; }
            /// <summary>
            /// State, province or whatever
            /// </summary>
            string Region { get; set; }

            /// <summary>
            /// Region code
            /// </summary>
            string RegionCode { get; set; }
            /// <summary>
            /// Timezone
            /// </summary>
            string TimeZone { get; set; }

            /// <summary>
            /// ZipCode when present
            /// </summary>
            string ZipCode { get; set; }
        }

        /// <summary>
        /// Header image
        /// </summary>
        [XmlElement("header-image", IsNullable = true)]
        public virtual string HeaderImage { get; set; } = OrigamiConstants.NoHeader;

        /// <summary>
        /// collection of localizations
        /// </summary>
        [XmlElement("localization", typeof(Localization))]
        public List<Localization> Localizations { get; set; }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Converts itself to string xml formed
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var t = GetType();
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(t);
            var swriter = new StringWriter(sb);
            using (swriter)
            {
                try
                {
                    serializer.Serialize(swriter, this);
                    return sb.ToString();
                }
                finally
                {
                    swriter.Close();
                }
            }
        }

        /// <summary>
        /// client tracking information
        /// </summary>
        public abstract class ClientTracking :
            IClientTracking,
            IVersion
        {
            /// <summary>
            /// public constructor
            /// </summary>
            public ClientTracking()
                : base()
            {
                DateCreated = System.DateTime.Now;
                DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Version = [];
            }

            /// <summary>
            /// Browser
            /// </summary>
            [XmlAttribute("browser")]
            [JsonPropertyName("browser")]
            public virtual string Browser { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlAttribute("crawler")]
            [JsonPropertyName("crawler")]
            public virtual bool Crawler { get; set; }

            /// <summary>
            /// Date and Time the View happened
            /// </summary>
            [XmlAttribute("dateCreated")]
            [JsonPropertyName("dateCreated")]
            public virtual DateTime DateCreated { get; set; }

            /// <summary>
            /// date and time (format: yyyy-MM-dd HH:mm)
            /// </summary>
            [XmlAttribute("dateTime")]
            [JsonPropertyName("dateTime")]
            public virtual string DateTime { get; set; }

            /// <summary>
            /// date and time converted to the appropriate type
            /// </summary>
            public virtual DateTime DateTime_
            {
                get
                {
                    var dtm = System.DateTime.MinValue;
                    System.DateTime.TryParse(DateTime, out dtm);
                    return dtm;
                }
            }

            /// <summary>
            /// host ip
            /// </summary>
			[XmlAttribute("hostAddress")]
            [JsonPropertyName("hostAddress")]
            public virtual string HostAddress { get; set; } = string.Empty;

            /// <summary>
            /// hostname
            /// </summary>
			[XmlAttribute("hostName")]
            [JsonPropertyName("hostName")]
            public virtual string HostName { get; set; } = string.Empty;

            /// <summary>
            /// Id
            /// </summary>
            [XmlAttribute("id")]
            [JsonPropertyName("id")]
            public virtual Guid Id { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("isBot")]
            [JsonPropertyName("isBot")]
            public virtual bool IsBot { get; set; }

            /// <summary>
            /// Is mobile device?
            /// </summary>
            [XmlAttribute("isMobileDevice")]
            [JsonPropertyName("isMobileDevice")]
            public virtual bool IsMobileDevice { get; set; }

            /// <inheritdoc/>
            [XmlElement("location")]
            [JsonPropertyName("location")]
            public virtual Location Location { get; set; } = new Location() { };

            /// <summary>
            /// Platform
            /// </summary>
            [XmlAttribute("platform")]
            [JsonPropertyName("platform")]
            public virtual string Platform { get; set; } = string.Empty;

            /// <summary>
            /// Row Id
            /// </summary>
            [XmlIgnore]
            [JsonIgnore]
            public virtual int RowId { get; set; }
            /// <inheritdoc/>
            [XmlAttribute("url")]
            [JsonPropertyName("url")]
            public virtual string Url { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlAttribute("urlReferrer")]
            [JsonPropertyName("urlReferrer")]
            public virtual string UrlReferrer { get; set; } = string.Empty;

            /// <summary>
            /// user agent
            /// </summary>
			[XmlAttribute("userAgent")]
            [JsonPropertyName("userAgent")]
            public virtual string UserAgent { get; set; } = string.Empty;

            /// <summary>
            /// user agent (100 characters)
            /// </summary>
			[XmlAttribute("userAgentAbbreviated")]
            [JsonPropertyName("userAgentAbbreviated")]
            public virtual string UserAgentAbbreviated
            {
                get
                {
                    if (UserAgent.Length >= 100)
                    {
                        return UserAgent?.Substring(0, 97) + "...";
                    }

                    return UserAgent;
                }
            }
            /// <summary>
            /// User profile Id
            /// </summary>
            [XmlAttribute("userProfile")]
            [JsonPropertyName("userProfile")]
            public virtual string UserProfile { get; set; } = string.Empty;
            /// <summary>
            /// Version or Row Timestamp
            /// </summary>
            [XmlAttribute("version")]
            [JsonPropertyName("version")]
            public virtual byte[] Version { get; set; }
        }

        /// <summary>
        /// Additional Info for Blogs
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForBlogs : AdditionalInfo
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public ForBlogs() : base()
            {

            }

            /// <summary>
            /// Order of the blog
            /// </summary>
            [XmlElement("order", IsNullable = true)]
            public int? Order { get; set; }
        }

        /// <summary>
        /// Additional info for categories
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForCategories : AdditionalInfo
        {
            /// <summary>
            /// constructor
            /// </summary>
            public ForCategories() : base() { }

            /// <summary>
            /// Image for category icon
            /// </summary>
            [XmlElement("image")]
            public new Image Image { get; set; } = Image.NoIconForCategories();

            /// <summary>
            /// Has Image?
            /// </summary>
            /// <returns></returns>
            public bool HasImage()
            {
                if (Image != null && string.IsNullOrWhiteSpace(Image.Source) == false) return true;
                return false;
            }
        }

        /// <summary>
        /// Additional Info for Comments
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForComments : AdditionalInfo
        {
            /// <summary>
            /// constructor
            /// </summary>
            public ForComments()
                : base()
            {
                Ratings = new Ratings();
                OriginalText = string.Empty;
                FirstContent = string.Empty;
            }

            /// <summary>
            /// first content to check if the comment was edited or modified
            /// </summary>
            [XmlElement("first-content", IsNullable = true)]
            public string FirstContent { get; set; }

            /// <summary>
            /// Original text with swearing and badwords
            /// </summary>
            [XmlElement("original-text", IsNullable = true)]
            public string OriginalText { get; set; }

            /// <summary>
            /// History of ratings
            /// </summary>
            [XmlElement("ratings")]
            public new Ratings Ratings { get; set; }
        }

        /// <summary>
        /// Additional Info for Pages
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForPages : AdditionalInfo, ILanguageWrittenOn
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public ForPages() : base()
            {
                LanguageWrittenOn = Thread.CurrentThread.CurrentUICulture.Name;
            }

            /// <summary>
            /// the language the post was written on
            /// </summary>
            [XmlElement("language-written-on", IsNullable = true)]
            public string LanguageWrittenOn { get; set; }
        }

        /// <summary>
        /// Additional info for Posts
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForPosts : AdditionalInfo, ILanguageWrittenOn
        {
            private const string keyHeader = "header-image";

            /// <summary>
            /// 
            /// </summary>
            public ForPosts()
                : base()
            {
                HeaderMedia = new HeaderMedia();
                LanguageWrittenOn = Thread.CurrentThread.CurrentUICulture.Name;
            }

            /// <summary>
            /// Post Header image
            /// </summary>
            [XmlIgnore]
            public override string HeaderImage
            {
                get
                {
                    var img = HeaderMedia.Images.FirstOrDefault(x => x.IsSocialMedia && x.Key == keyHeader) ?? new();
                    return img.Source.NoHeader();
                }
                set
                {
                    var img = HeaderMedia.Images.FirstOrDefault(x => x.IsSocialMedia && x.Key == keyHeader)
                        ?? new Image { Key = keyHeader, IsSocialMedia = true, };

                    img.Source = value;

                    if (HeaderMedia.Images.Contains(img) == false) HeaderMedia.Images.Add(img);
                }
            }

            /// <summary>
            /// Header media
            /// </summary>
            [XmlElement("headerMedia")]
            public new HeaderMedia HeaderMedia { get; set; }

            /// <summary>
            /// the language the post was written on
            /// </summary>
            [XmlElement("language-written-on", IsNullable = true)]
            public string LanguageWrittenOn { get; set; }
        }

        /// <summary>
        /// Additional Info for Pages
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForSitePages : AdditionalInfo, ILanguageWrittenOn
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public ForSitePages() : base()
            {
                LanguageWrittenOn = Thread.CurrentThread.CurrentUICulture.Name;
            }

            /// <summary>
            /// the language the post was written on
            /// </summary>
            [XmlElement("language-written-on", IsNullable = true)]
            public string LanguageWrittenOn { get; set; }
        }

        /// <summary>
        /// Additional info for Users
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForUsers : AdditionalInfo, IFirstName, ILastName, IFacebook, IInstagram, IGitHub, IPersonalWebsite, ILinkedIn, ITOTPSecret
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public ForUsers() : base()
            {
                DisplayName = string.Empty;
                Facebook = string.Empty;
                FirstName = string.Empty;
                GitHub = string.Empty;
                HeaderImage = OrigamiConstants.NoUser;
                Instagram = string.Empty;
                LastName = string.Empty;
                LinkedIn = string.Empty;
                TOTPSecret = string.Empty;
                Website = string.Empty;
                TOTPRecoveryCodes = string.Empty;
            }

            [XmlElement("display-name", IsNullable = true)]
            public string DisplayName { get; set; }

            [XmlElement("facebook", IsNullable = true)]
            public string Facebook { get; set; }

            [XmlElement("first-name", IsNullable = true)]
            public string FirstName { get; set; }

            [XmlElement("github", IsNullable = true)]
            public string GitHub { get; set; }

            [XmlElement("instagram", IsNullable = true)]
            public string Instagram { get; set; }

            [XmlElement("last-name", IsNullable = true)]
            public string LastName { get; set; }

            [XmlElement("website", IsNullable = true)]
            public string Website { get; set; }

            [XmlElement("linkedin", IsNullable = true)]
            public string LinkedIn { get; set; }

            [XmlElement("totp-secret", IsNullable = false)]
            public string TOTPSecret { get; set; }

            [XmlElement("totp-recovery-codes", IsNullable = true)]
            public string TOTPRecoveryCodes { get; set; }
        }

        /// <summary>
        /// Additional info for Videos
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForVideos : AdditionalInfo, ILanguageWrittenOn, IHeaderImage, IEmbedIFrame
        {
            private const string keyHeader = "header-image";

            /// <summary>
            /// Default constructor
            /// </summary>
            public ForVideos() : base()
            {
                HeaderMedia = new HeaderMedia();
                LanguageWrittenOn = Thread.CurrentThread.CurrentUICulture.Name;
                EmbedIFrame = string.Empty;
            }

            /// <summary>
            /// Post Header image
            /// </summary>
            [XmlIgnore]
            public override string HeaderImage
            {
                get
                {
                    var img = HeaderMedia.Images.FirstOrDefault(x => x.IsSocialMedia && x.Key == keyHeader);
                    return img != null ? img.Source.NoHeader() : OrigamiConstants.NoHeader;
                }
                set
                {
                    var img = HeaderMedia.Images.FirstOrDefault(x => x.IsSocialMedia && x.Key == keyHeader) ?? new Image { Key = keyHeader, IsSocialMedia = true, };
                    img.Source = value;
                    if (HeaderMedia.Images.Contains(img) == false) HeaderMedia.Images.Add(img);
                }
            }

            /// <summary>
            /// Header media
            /// </summary>
            [XmlElement("headerMedia")]
            public new HeaderMedia HeaderMedia { get; set; }

            [XmlElement("language-written-on", IsNullable = true)]
            public string LanguageWrittenOn { get; set; }

            [XmlElement("embed-iframe", IsNullable = true)]
            public string EmbedIFrame { get; set; }
        }

        /// <summary>
        /// Additional info for Videos
        /// </summary>
        [XmlRoot("additionalInfo")]
        public class ForContents : AdditionalInfo, ILanguageWrittenOn, IHeaderImage, IEmbedIFrame
        {
            private const string keyHeader = "header-image";

            /// <summary>
            /// Default constructor
            /// </summary>
            public ForContents() : base()
            {
                HeaderMedia = new HeaderMedia();
                LanguageWrittenOn = Thread.CurrentThread.CurrentUICulture.Name;
                EmbedIFrame = string.Empty;
            }

            /// <summary>
            /// Post Header image
            /// </summary>
            [XmlIgnore]
            public override string HeaderImage
            {
                get
                {
                    var img = HeaderMedia.Images.FirstOrDefault(x => x.IsSocialMedia && x.Key == keyHeader);
                    return img != null ? img.Source.NoHeader() : OrigamiConstants.NoHeader;
                }
                set
                {
                    var img = HeaderMedia.Images.FirstOrDefault(x => x.IsSocialMedia && x.Key == keyHeader) ?? new Image { Key = keyHeader, IsSocialMedia = true, };
                    img.Source = value;
                    if (HeaderMedia.Images.Contains(img) == false) HeaderMedia.Images.Add(img);
                }
            }

            /// <summary>
            /// Header media
            /// </summary>
            [XmlElement("headerMedia")]
            public new HeaderMedia HeaderMedia { get; set; }

            [XmlElement("language-written-on", IsNullable = true)]
            public string LanguageWrittenOn { get; set; }

            [XmlElement("embed-iframe", IsNullable = true)]
            public string EmbedIFrame { get; set; }
        }

        /// <summary>
        /// Header media for Additional Info carriers
        /// </summary>
        [Serializable]
        [XmlRoot("headerMedia")]
        public class HeaderMedia
        {
            public HeaderMedia()
            {
                YoutubeVideos = new List<Youtube>();
                DefaultVideos = new List<Video>();
                Images = new List<Image>();
            }

            [XmlElement("video", typeof(Video))]
            public List<Video> DefaultVideos { get; set; }

            [XmlElement("image", typeof(Image))]
            public List<Image> Images { get; set; }

            [XmlElement("youtube", typeof(Youtube))]
            public List<Youtube> YoutubeVideos { get; set; }
        }

        [Serializable]
        [XmlRoot("image")]
        public class Image : WebResource
        {
            /// <summary>
            /// default constructor
            /// </summary>
            public Image() : base() { Source = string.Empty; }

            /// <summary>
            /// Source Attribute for this Image
            /// </summary>
            [XmlAttribute("src")]
            public string Source { get; set; }

            /// <summary>
            /// No icon for categories
            /// </summary>
            public static Image NoIconForCategories() => new Image() { Source = OrigamiConstants.NoCategory };
        }

        [Serializable]
        [XmlRoot("localization")]
        public class Localization
        {
            /// <summary>
            /// culture info ISO code
            /// </summary>
            [XmlAttribute("culture-info-isocode")]
            public string CultureInfo { get; set; } = string.Empty;

            /// <summary>
            /// key for this localization
            /// </summary>
            [XmlAttribute("key")]
            public string Key { get; set; } = string.Empty;

            /// <summary>
            /// translation for it
            /// </summary>
            [XmlText]
            public string Translation { get; set; } = string.Empty;
            /// <summary>
            /// Returns translation string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Translation.Trim();
            }
        }
        /// <summary>
        /// Location class
        /// </summary>
        [XmlRoot("location")]
        public class Location : ILocation
        {
            /// <inheritdoc/>
            [XmlElement("city")]
            [JsonPropertyName("city")]
            public virtual string City { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlElement("country")]
            [JsonPropertyName("country")]
            public virtual string Country { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlElement("countryCode")]
            [JsonPropertyName("countryCode")]
            public virtual string CountryCode { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlElement("ip")]
            [JsonPropertyName("ip")]
            public virtual string Ip { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlElement("latitude")]
            [JsonPropertyName("latitude")]
            public virtual float? Latitude { get; set; }

            /// <inheritdoc/>
            [XmlElement("longitude")]
            [JsonPropertyName("longitude")]
            public virtual float? Longitude { get; set; }
            /// <inheritdoc/>
            [XmlElement("region")]
            [JsonPropertyName("region")]
            public virtual string Region { get; set; } = string.Empty;
            /// <inheritdoc/>
            [XmlElement("regionCode")]
            [JsonPropertyName("regionCode")]
            public virtual string RegionCode { get; set; } = string.Empty;
            /// <inheritdoc/>
            [XmlElement("timeZone")]
            [JsonPropertyName("timeZone")]
            public virtual string TimeZone { get; set; } = string.Empty;

            /// <inheritdoc/>
            [XmlElement("zipCode")]
            [JsonPropertyName("zipCode")]
            public virtual string ZipCode { get; set; } = string.Empty;
        }

        [Serializable]
        [XmlRoot("rating")]
        public class Rating : ClientTracking
        {
            /// <summary>
            /// 
            /// </summary>
            [XmlAttribute("rating-level")]
            public float Rate { get; set; }
        }

        [Serializable]
        [XmlRoot("ratings")]
        public class Ratings
        {
            public Ratings()
            {
                RatingHistory = new List<Rating>();
            }

            /// <summary>
            /// History of ratings
            /// </summary>
            [XmlElement("rating", typeof(Rating))]
            public List<Rating> RatingHistory { get; set; }
        }

        [Serializable]
        [XmlRoot("video")]
        public class Video : WebResource
        {

        }

        /// <summary>
        /// web resource xml definition for additional info
        /// </summary>
        [Serializable]
        public abstract class WebResource
        {
            /// <summary>
            /// this resource can be used in social network as social media
            /// </summary>
            [XmlAttribute("is-social-media")]
            public bool IsSocialMedia { get; set; }

            /// <summary>
            /// key for this resource
            /// </summary>
            [XmlAttribute("key")]
            public string Key { get; set; } = string.Empty;
        }
        /// <summary>
        /// YouTube definition for additional information
        /// </summary>
        [Serializable]
        [XmlRoot("youtube")]
        public class Youtube : Video
        {
            /// <summary>
            /// youtube id for this video
            /// </summary>
            [XmlAttribute("v")]
            public string Source { get; set; } = string.Empty;
        }
    }
}
