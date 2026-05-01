using Origami.Core.Models.Settings;
using System.ComponentModel;

namespace Origami.Core.Models
{
    /// <summary>
    /// Represents the configured settings for the blog engine. 
    /// Settings actually come from the oi_Settings table, so no table attribute for this class.
    /// </summary>
    public class OrigamiSettings :
        IChanged,
        IId,
        INew,
        IHeaderImage,
        ILanguageWrittenOn
    {
        public OrigamiSettings() : base()
        {

        }

        #region PRIVATE/PROTECTED/PUBLIC MEMBERS

        private const int _defaultMaxRemoteFileSize = 524288;

        private const int _defaultRemoteDownloadTimeout = 30000;

        private bool _allowServerToDownloadRemoteFiles;

        private string _alternateFeedUrl = string.Empty;

        private string _authorName = string.Empty;

        private string _avatar = string.Empty;

        private int _blogrollMaxLength;

        private int _blogrollUpdateMinutes;

        private int _blogrollVisiblePosts;

        private CommentsBy _commentProvider;

        private bool _commentReportMistakes;

        /// <summary>
        ///     The number of comments per page.
        /// </summary>
        private int _commentsPerPage;

        private bool _compressWebResource;

        private string _contactErrorMessage = string.Empty;

        private string _contactFormMessage = string.Empty;

        private string _contactThankMessage = string.Empty;

        private bool _createBlogOnSelfRegistration;

        private string _culture = string.Empty;

        private int _daysCommentsAreEnabled;

        private string _description = string.Empty;
        private int _descriptionCharacters;
        private int _descriptionCharactersForPostsByTagOrCategory;
        private bool _disqusAddCommentsToPages;
        private bool _disqusDevMode;
        private string _disqusWebsiteName = string.Empty;
        private string _email = string.Empty;
        private string _emailSubjectPrefix = string.Empty;
        private bool _enableCommentSearch;
        private bool _enableCommentsModeration;
        private bool _enableContactAttachments;
        private bool _enableCountryInComments;
        private bool _enableEnclosures;
        /// <summary>
        ///     The enable http compression.
        /// </summary>
        private bool _enableHttpCompression;

        private bool _enableOpenSearch;
        private bool _enableOptimization;
        /// <summary>
        ///     Whether passwords can be reset.
        /// </summary>
        private bool _enablePasswordResets = true;

        private bool _enablePingBackReceive;
        private bool _enablePingBackSend;
        private bool _enableRating;
        private bool _enableRecaptchaOnContactForm;
        private bool _enableReferrerTracking;
        private bool _enableRelatedPosts;
        private bool _enableSelfRegistration;
        private bool _enableSsl;
        private bool _enableTagExport;
        private bool _enableTrackBackReceive;
        private bool _enableTrackBackSend;
        private bool _enableWebsiteInComments;
        private string _endorsement = string.Empty;
        private string _errorText = string.Empty;
        private string _errorTitle = string.Empty;
        private string _facebookAppId = string.Empty;
        private string _facebookLanguage = string.Empty;
        private string _feedAuthor = string.Empty;
        private float _geocodingLatitude;
        private float _geocodingLongitude;
        private string _handleWwwSubdomain = string.Empty;
        private string _htmlHeader = string.Empty;
        private bool _isCoCommentEnabled;
        private bool _isCommentNestingEnabled;
        private bool _isCommentsEnabled;
        private bool _keepTestingTheSmtpServerConnectivity = false;
        private string _languageWrittenOn = string.Empty;
        private DateTime _lastDatabaseMigration = DateTime.MinValue;
        private bool _maintenanceMode = false;
        private int _maxRemoteFileSize = _defaultMaxRemoteFileSize;
        private string _name = string.Empty;
        private int _numberOfReferrerDays;
        private OpenTelemetry _openTelemetry = new();
        private bool _pageOptionsCustomFields;
        private bool _pageOptionsDescription;
        private bool _pageOptionsSlug;
        private bool _postOptionsCustomFields;
        private bool _postOptionsDescription;
        private bool _postOptionsSlug;
        private int _postsPerFeed;
        private int _postsPerPage;
        private bool _redirectToRemoveFileExtension;
        /// <summary>
        /// The timeout in milliseconds for a remote download. Default is 30 seconds.
        /// </summary>
        private int _remoteDownloadTimeout = _defaultRemoteDownloadTimeout;

        private bool _requireSslMetaWeblogApi;
        private string _searchButtonText = string.Empty;
        private string _searchCommentLabelText = string.Empty;
        private string _searchDefaultText = string.Empty;
        private string _securityValidationKey = string.Empty;
        private string _selfRegistrationInitialRole = string.Empty;
        private bool _sendMailOnComment;
        private bool _showDescriptionInPostList;
        private bool _showDescriptionInPostListForPostsByTagOrCategory;
        private bool _showIncludeCommentsOption;
        private bool _showLivePreview;
        private bool _showPostNavigation;
        private string _smtpPassword = string.Empty;
        private string _smtpServer = string.Empty;
        private int _smtpServerPort;
        private string _smtpUserName = string.Empty;
        private SocialNetwork _socialNetwork = new();
        private string _syndicationFormat = string.Empty;
        private bool _timeStampPostLinks;
        private string _timeZoneId = string.Empty;
        private string _trackingScript = string.Empty;
        private bool _trustAuthenticatedUsers;
        private bool _useBlogNameInPageTitles;
        /// <summary>
        ///     Occurs when [changed].
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        #endregion

        #region Description

        /// <summary>
        ///     Gets or sets the description of the blog.
        /// </summary>
        /// <value>A brief synopsis of the blog content.</value>
        /// <remarks>
        ///     This value is also used for the description meta tag.
        /// </remarks>
        public string Description
        {
            get => _description;
            set => this.Set(ref _description, value, Changed);
        }

        #endregion

        #region EnableHttpCompression

        /// <summary>
        ///     Gets or sets a value indicating if HTTP compression is enabled.
        /// </summary>
        /// <value><b>true</b> if compression is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableHttpCompression
        {
            get => _enableHttpCompression;
            set => this.Set(ref _enableHttpCompression, value, Changed);
        }

        #endregion

        #region EnableReferrerTracking

        /// <summary>
        ///     Gets or sets a value indicating if referral tracking is enabled.
        /// </summary>
        /// <value><b>true</b> if referral tracking is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableReferrerTracking
        {
            get => _enableReferrerTracking;
            set => this.Set(ref _enableReferrerTracking, value, Changed);
        }

        #endregion

        #region NumberOfReferrerDays

        /// <summary>
        ///     Gets or sets a value indicating the number of days that referrer information should be stored.
        /// </summary>
        public int NumberOfReferrerDays
        {
            get => _numberOfReferrerDays;
            set => this.Set(ref _numberOfReferrerDays, value, Changed);
        }

        #endregion

        #region EnableRelatedPosts

        /// <summary>
        ///     Gets or sets a value indicating if related posts are displayed.
        /// </summary>
        /// <value><b>true</b> if related posts are displayed, otherwise returns <b>false</b>.</value>
        public bool EnableRelatedPosts
        {
            get => _enableRelatedPosts;
            set => this.Set(ref _enableRelatedPosts, value, Changed);
        }

        #endregion

        #region AlternateFeedUrl

        /// <summary>
        ///     Gets or sets the FeedBurner user name.
        /// </summary>
        public string AlternateFeedUrl
        {
            get => _alternateFeedUrl;
            set => this.Set(ref _alternateFeedUrl, value, Changed);
        }

        #endregion

        #region FeedAuthor

        /// <summary>
        /// RSS feed author
        /// </summary>
        public string FeedAuthor
        {
            get => _feedAuthor;
            set => this.Set(ref _feedAuthor, value, Changed);
        }

        #endregion

        #region TimeStampPostLinks

        /// <summary>
        ///     Gets or sets whether or not to time stamp post links.
        /// </summary>
        public bool TimeStampPostLinks
        {
            get => _timeStampPostLinks;
            set => this.Set(ref _timeStampPostLinks, value, Changed);
        }

        #endregion

        #region Name

        /// <summary>
        ///     Gets or sets the name of the blog.
        /// </summary>
        /// <value>The title of the blog.</value>
        public string Name
        {
            get => _name;
            set => this.Set(ref _name, value, Changed);
        }

        #endregion

        #region PostsPerPage

        /// <summary>
        ///     Gets or sets the number of posts to show an each page.
        /// </summary>
        /// <value>The number of posts to show an each page.</value>
        public int PostsPerPage
        {
            get => _postsPerPage;
            set => this.Set(ref _postsPerPage, value, Changed);
        }

        #endregion

        #region ShowLivePreview

        /// <summary>
        ///     Gets or sets a value indicating if live preview of post is displayed.
        /// </summary>
        /// <value><b>true</b> if live previews are displayed, otherwise returns <b>false</b>.</value>
        public bool ShowLivePreview
        {
            get => _showLivePreview;
            set => this.Set(ref _showLivePreview, value, Changed);
        }

        #endregion

        #region EnableRating

        /// <summary>
        ///     Gets or sets a value indicating if live preview of post is displayed.
        /// </summary>
        /// <value><b>true</b> if live previews are displayed, otherwise returns <b>false</b>.</value>
        public bool EnableRating
        {
            get => _enableRating;
            set => this.Set(ref _enableRating, value, Changed);
        }

        #endregion

        #region ShowDescriptionInPostList

        /// <summary>
        ///     Gets or sets a value indicating if the full post is displayed in lists or only the description/excerpt.
        /// </summary>
        public bool ShowDescriptionInPostList
        {
            get => _showDescriptionInPostList;
            set => this.Set(ref _showDescriptionInPostList, value, Changed);
        }

        #endregion

        #region DescriptionCharacters

        /// <summary>
        ///     Gets or sets a value indicating how many characters should be shown of the description
        /// </summary>
        public int DescriptionCharacters
        {
            get => _descriptionCharacters;
            set => this.Set(ref _descriptionCharacters, value, Changed);
        }

        #endregion

        #region ShowDescriptionInPostListForPostsByTagOrCategory

        /// <summary>
        ///     Gets or sets a value indicating if the full post is displayed in lists by tag/category or only the description/excerpt.
        /// </summary>
        public bool ShowDescriptionInPostListForPostsByTagOrCategory
        {
            get => _showDescriptionInPostListForPostsByTagOrCategory;
            set => this.Set(ref _showDescriptionInPostListForPostsByTagOrCategory, value, Changed);
        }

        #endregion

        #region DescriptionCharactersForPostsByTagOrCategory

        /// <summary>
        ///     Gets or sets a value indicating how many characters should be shown of the description when posts are shown by tag or category.
        /// </summary>
        public int DescriptionCharactersForPostsByTagOrCategory
        {
            get => _descriptionCharactersForPostsByTagOrCategory;
            set => this.Set(ref _descriptionCharactersForPostsByTagOrCategory, value, Changed);
        }

        #endregion

        #region Enclosure support

        /// <summary>
        ///     Enable enclosures for RSS feeds
        /// </summary>
        public bool EnableEnclosures
        {
            get => _enableEnclosures;
            set => this.Set(ref _enableEnclosures, value, Changed);
        }

        #endregion

        #region Tags Export

        /// <summary>
        ///     Enable exporting of tags in the RSS syndication feed.
        /// </summary>
        public bool EnableTagExport
        {
            get => _enableTagExport;
            set => this.Set(ref _enableTagExport, value, Changed);
        }

        #endregion

        #region SyndicationFormat

        /// <summary>
        ///     Gets or sets the default syndication format used by the blog.
        /// </summary>
        /// <value>The default syndication format used by the blog.</value>
        /// <remarks>
        ///     If no value is specified, blog defaults to using RSS 2.0 format.
        /// </remarks>
        public string SyndicationFormat
        {
            get => _syndicationFormat;
            set => this.Set(ref _syndicationFormat, value, Changed);
        }

        #endregion

        #region CompressWebResource

        /// <summary>
        ///     Gets or sets a value indicating whether to compress WebResource.axd
        /// </summary>
        /// <value><c>true</c> if [compress web resource]; otherwise, <c>false</c>.</value>
        public bool CompressWebResource
        {
            get => _compressWebResource;
            set => this.Set(ref _compressWebResource, value, Changed);
        }

        #endregion

        #region EnableOptimization

        /// <summary>
        ///     DO NOT USE: no longer needed and will be removed in later versions
        /// </summary>
        public bool EnableOptimization
        {
            get => _enableOptimization;
            set => this.Set(ref _enableOptimization, value, Changed);
        }

        #endregion 

        #region UseBlogNameInPageTitles

        /// <summary>
        ///     Gets or sets a value indicating if whitespace in stylesheets should be removed
        /// </summary>
        /// <value><b>true</b> if whitespace is removed, otherwise returns <b>false</b>.</value>
        public bool UseBlogNameInPageTitles
        {
            get => _useBlogNameInPageTitles;
            set => this.Set(ref _useBlogNameInPageTitles, value, Changed);
        }

        #endregion

        #region RequireSSLMetaWeblogAPI;

        /// <summary>
        ///     Gets or sets a value indicating whether [require SSL for MetaWeblogAPI connections].
        /// </summary>
        public bool RequireSslMetaWeblogApi
        {
            get => _requireSslMetaWeblogApi;
            set => this.Set(ref _requireSslMetaWeblogApi, value, Changed);
        }

        #endregion

        #region EnableOpenSearch

        /// <summary>
        ///     Gets or sets a value indicating if whitespace in stylesheets should be removed
        /// </summary>
        /// <value><b>true</b> if whitespace is removed, otherwise returns <b>false</b>.</value>
        public bool EnableOpenSearch
        {
            get => _enableOpenSearch;
            set => this.Set(ref _enableOpenSearch, value, Changed);
        }

        #endregion

        #region TrackingScript

        /// <summary>
        ///     Gets or sets the tracking script used to collect visitor data.
        /// </summary>
        public string TrackingScript
        {
            get => _trackingScript;
            set => this.Set(ref _trackingScript, value, Changed);
        }

        #endregion

        #region ShowPostNavigation

        /// <summary>
        ///     Gets or sets a value indicating whether or not to show the post navigation.
        /// </summary>
        /// <value><c>true</c> if [show post navigation]; otherwise, <c>false</c>.</value>
        public bool ShowPostNavigation
        {
            get => _showPostNavigation;
            set => this.Set(ref _showPostNavigation, value, Changed);
        }

        #endregion

        #region EnablePasswordReset

        /// <summary>
        ///     Gets or sets a value indicating whether or not to enable password resets.
        /// </summary>
        /// <value><c>true</c> if [enable self registration]; otherwise, <c>false</c>.</value>
        public bool EnablePasswordReset
        {
            get { return _enablePasswordResets; }
            set { this.Set(ref _enablePasswordResets, value, Changed); }
        }

        #endregion

        #region SelfRegistration

        /// <summary>
        /// If we need to create blog for self-registered user
        /// (instead of just add user to existing blog)
        /// </summary>
        public bool CreateBlogOnSelfRegistration
        {
            get => _createBlogOnSelfRegistration;
            set => this.Set(ref _createBlogOnSelfRegistration, value, Changed);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not to enable self registration.
        /// </summary>
        /// <value><c>true</c> if [enable self registration]; otherwise, <c>false</c>.</value>
        public bool EnableSelfRegistration
        {
            get => _enableSelfRegistration;
            set => this.Set(ref _enableSelfRegistration, value, Changed);
        }

        /// <summary>
        ///     Gets or sets the initial role assigned to users who self register.
        /// </summary>
        /// <value>The role name.</value>
        public string SelfRegistrationInitialRole
        {
            get => _selfRegistrationInitialRole;
            set => this.Set(ref _selfRegistrationInitialRole, value, Changed);
        }
        #endregion

        #region HandleWwwSubdomain

        /// <summary>
        ///     Gets or sets how to handle the www subdomain of the url (for SEO purposes).
        /// </summary>
        public string HandleWwwSubdomain
        {
            get => _handleWwwSubdomain;
            set => this.Set(ref _handleWwwSubdomain, value, Changed);
        }

        #endregion

        #region EnablePingBackSend

        /// <summary>
        ///     Gets or sets a value indicating whether [enable ping back send].
        /// </summary>
        /// <value><c>true</c> if [enable ping back send]; otherwise, <c>false</c>.</value>
        public bool EnablePingBackSend
        {
            get => _enablePingBackSend;
            set => this.Set(ref _enablePingBackSend, value, Changed);
        }

        #endregion

        #region EnablePingBackReceive;

        /// <summary>
        ///     Gets or sets a value indicating whether [enable ping back receive].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable ping back receive]; otherwise, <c>false</c>.
        /// </value>
        public bool EnablePingBackReceive
        {
            get => _enablePingBackReceive;
            set => this.Set(ref _enablePingBackReceive, value, Changed);
        }

        #endregion

        #region EnableTrackBackSend;

        /// <summary>
        ///     Gets or sets a value indicating whether [enable track back send].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable track back send]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTrackBackSend
        {
            get => _enableTrackBackSend;
            set => this.Set(ref _enableTrackBackSend, value, Changed);
        }

        #endregion

        #region EnableTrackBackReceive;

        /// <summary>
        ///     Gets or sets a value indicating whether [enable track back receive].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable track back receive]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTrackBackReceive
        {
            get => _enableTrackBackReceive;
            set => this.Set(ref _enableTrackBackReceive, value, Changed);
        }

        #endregion

        #region Email

        /// <summary>
        ///     Gets or sets the e-mail address notifications are sent to.
        /// </summary>
        /// <value>The e-mail address notifications are sent to.</value>
        public string Email
        {
            get => _email;
            set => this.Set(ref _email, value, Changed);
        }

        #endregion

        #region SendMailOnComment

        /// <summary>
        ///     Gets or sets a value indicating if an enail is sent when a comment is added to a post.
        /// </summary>
        /// <value><b>true</b> if email notification of new comments is enabled, otherwise returns <b>false</b>.</value>
        public bool SendMailOnComment
        {
            get => _sendMailOnComment;
            set => this.Set(ref _sendMailOnComment, value, Changed);
        }

        #endregion

        #region SmtpSettings

        /// <summary>
        ///     Gets or sets a value indicating if SSL is enabled for sending e-mails
        /// </summary>
        public bool EnableSsl
        {
            get => _enableSsl;
            set => this.Set(ref _enableSsl, value, Changed);
        }

        public bool KeepTestingTheSmtpServerConnectivity
        {
            get => _keepTestingTheSmtpServerConnectivity;
            set => this.Set(ref _keepTestingTheSmtpServerConnectivity, value, Changed);
        }

        /// <summary>
        ///     Gets or sets the password used to connect to the SMTP server.
        /// </summary>
        /// <value>The password used to connect to the SMTP server.</value>
        public string SmtpPassword
        {
            get => _smtpPassword;
            set => this.Set(ref _smtpPassword, value, Changed);
        }

        /// <summary>
        ///     Gets or sets the DNS name or IP address of the SMTP server used to send notification emails.
        /// </summary>
        /// <value>The DNS name or IP address of the SMTP server used to send notification emails.</value>
        public string SmtpServer
        {
            get => _smtpServer;
            set => this.Set(ref _smtpServer, value, Changed);
        }

        /// <summary>
        ///     Gets or sets the DNS name or IP address of the SMTP server used to send notification emails.
        /// </summary>
        /// <value>The DNS name or IP address of the SMTP server used to send notification emails.</value>
        public int SmtpServerPort
        {
            get => _smtpServerPort;
            set => this.Set(ref _smtpServerPort, value, Changed);
        }

        /// <summary>
        ///     Gets or sets the user name used to connect to the SMTP server.
        /// </summary>
        /// <value>The user name used to connect to the SMTP server.</value>
        public string SmtpUserName
        {
            get => _smtpUserName;
            set => this.Set(ref _smtpUserName, value, Changed);
        }

        #endregion

        #region EmailSubjectPrefix

        /// <summary>
        ///     Gets or sets the email subject prefix.
        /// </summary>
        /// <value>The email subject prefix.</value>
        public string EmailSubjectPrefix
        {
            get => _emailSubjectPrefix;
            set => this.Set(ref _emailSubjectPrefix, value, Changed);
        }

        #endregion

        #region DaysCommentsAreEnabled

        /// <summary>
        ///     Gets or sets the number of days that a post accepts comments.
        /// </summary>
        /// <value>The number of days that a post accepts comments.</value>
        /// <remarks>
        ///     After this time period has expired, comments on a post are disabled.
        /// </remarks>
        public int DaysCommentsAreEnabled
        {
            get => _daysCommentsAreEnabled;
            set => this.Set(ref _daysCommentsAreEnabled, value, Changed);
        }

        #endregion

        #region EnableCountryInComments

        /// <summary>
        ///     Gets or sets a value indicating if dispay of the country of commenter is enabled.
        /// </summary>
        /// <value><b>true</b> if commenter country display is enabled, otherwise returns <b>false</b>.</value>
        public bool EnableCountryInComments
        {
            get => _enableCountryInComments;
            set => this.Set(ref _enableCountryInComments, value, Changed);
        }

        #endregion

        #region EnableWebsiteInComments

        /// <summary>
        ///     Gets or sets a value indicating if display of the website of commenter is enabled
        /// </summary>
        public bool EnableWebsiteInComments
        {
            get => _enableWebsiteInComments;
            set => this.Set(ref _enableWebsiteInComments, value, Changed);
        }

        #endregion

        #region IsCommentsEnabled

        /// <summary>
        ///     Gets or sets a value indicating if comments are enabled for posts.
        /// </summary>
        /// <value><b>true</b> if comments can be made against a post, otherwise returns <b>false</b>.</value>
        public bool IsCommentsEnabled
        {
            get => _isCommentsEnabled;
            set => this.Set(ref _isCommentsEnabled, value, Changed);
        }

        #endregion

        #region IsCoCommentEnabled

        /// <summary>
        ///     Only here so old themes won't break
        /// </summary>
        /// <value>false</value>
        public bool IsCoCommentEnabled
        {
            get => _isCoCommentEnabled;
            set => this.Set(ref _isCoCommentEnabled, value, Changed);
        }

        #endregion

        #region Avatar

        /// <summary>
        ///     Gets or sets a value indicating if Gravatars are enabled or not.
        /// </summary>
        /// <value><b>true</b> if Gravatars are enabled, otherwise returns <b>false</b>.</value>
        public string Avatar
        {
            get => _avatar;
            set => this.Set(ref _avatar, value, Changed);
        }

        #endregion

        #region IsCommentNestingEnabled

        /// <summary>
        ///     Gets or sets a value indicated if comments should be displayed as nested.
        /// </summary>
        /// <value><b>true</b> if comments should be displayed as nested, <b>false</b> for flat comments.</value>
        public bool IsCommentNestingEnabled
        {
            get => _isCommentNestingEnabled;
            set => this.Set(ref _isCommentNestingEnabled, value, Changed);
        }

        #endregion

        #region Trust authenticated users

        ///<summary>
        ///    If true comments from authenticated users always approved
        ///</summary>
        public bool TrustAuthenticatedUsers
        {
            get => _trustAuthenticatedUsers;
            set => this.Set(ref _trustAuthenticatedUsers, value, Changed);
        }

        #endregion

        #region SecurityValidationKey

        /// <summary>
        ///     Gets or sets the security validation key.
        /// </summary>
        /// <value>The security validation key.</value>
        public string SecurityValidationKey
        {
            get => _securityValidationKey;
            set => this.Set(ref _securityValidationKey, value, Changed);
        }

        #endregion

        #region Comments per page

        /// <summary>
        ///     Number of comments per page displayed in the comments admin section
        /// </summary>
        public int CommentsPerPage
        {
            get { return Math.Max(_commentsPerPage, 5); }
            set { this.Set(ref _commentsPerPage, value, Changed); }
        }

        #endregion

        #region Comment providers and moderation

        /// <summary>
        /// Comments provider
        /// </summary>
        public enum CommentsBy
        {
            /// <summary>
            ///     Internal BlogEngine comments
            /// </summary>
            BlogEngine = 0,
            /// <summary>
            ///     Comments by Disqus
            /// </summary>
            Disqus = 1,
            /// <summary>
            ///     Comments by Facebook
            /// </summary>
            Facebook = 2
        }

        /// <summary>
        ///     Gets or sets a value indicating comment provider
        /// </summary>
        public CommentsBy CommentProvider
        {
            get => _commentProvider;
            set => this.Set(ref _commentProvider, value, Changed);
        }

        /// <summary>
        ///     Enables to report mistakes back to service
        /// </summary>
        public bool CommentReportMistakes
        {
            get => _commentReportMistakes;
            set => this.Set(ref _commentReportMistakes, value, Changed);
        }

        /// <summary>
        ///     Allow also to add comments to the pages
        /// </summary>
        public bool DisqusAddCommentsToPages
        {
            get => _disqusAddCommentsToPages;
            set => this.Set(ref _disqusAddCommentsToPages, value, Changed);
        }

        /// <summary>
        ///     Development mode to test disqus on local host
        /// </summary>
        public bool DisqusDevMode
        {
            get => _disqusDevMode;
            set => this.Set(ref _disqusDevMode, value, Changed);
        }

        /// <summary>
        ///     Short website name that used to identify Disqus account
        /// </summary>
        public string DisqusWebsiteName
        {
            get => _disqusWebsiteName;
            set => this.Set(ref _disqusWebsiteName, value, Changed);
        }

        /// <summary>
        ///     Gets or sets a value indicating if comments moderation is used for posts.
        /// </summary>
        /// <value><b>true</b> if comments are moderated for posts, otherwise returns <b>false</b>.</value>
        public bool EnableCommentsModeration
        {
            get => _enableCommentsModeration;
            set => this.Set(ref _enableCommentsModeration, value, Changed);
        }
        /// <summary>
        /// Facebook application ID
        /// </summary>
        public string FacebookAppId
        {
            get => _facebookAppId;
            set => this.Set(ref _facebookAppId, value, Changed);
        }

        /// <summary>
        /// Facebook language
        /// </summary>
        public string FacebookLanguage
        {
            get => _facebookLanguage;
            set => this.Set(ref _facebookLanguage, value, Changed);
        }

        #endregion

        #region BlogrollMaxLength

        /// <summary>
        ///     Gets or sets the maximum number of characters that are displayed from a blog-roll retrieved post.
        /// </summary>
        /// <value>The maximum number of characters to display.</value>
        public int BlogrollMaxLength
        {
            get => _blogrollMaxLength;
            set => this.Set(ref _blogrollMaxLength, value, Changed);
        }

        #endregion

        #region BlogrollUpdateMinutes

        /// <summary>
        ///     Gets or sets the number of minutes to wait before polling blog-roll sources for changes.
        /// </summary>
        /// <value>The number of minutes to wait before polling blog-roll sources for changes.</value>
        public int BlogrollUpdateMinutes
        {
            get => _blogrollUpdateMinutes;
            set => this.Set(ref _blogrollUpdateMinutes, value, Changed);
        }

        #endregion

        #region BlogrollVisiblePosts

        /// <summary>
        ///     Gets or sets the number of posts to display from a blog-roll source.
        /// </summary>
        /// <value>The number of posts to display from a blog-roll source.</value>
        public int BlogrollVisiblePosts
        {
            get => _blogrollVisiblePosts;
            set => this.Set(ref _blogrollVisiblePosts, value, Changed);
        }

        #endregion

        #region EnableCommentSearch

        /// <summary>
        ///     Gets or sets a value indicating if search of post comments is enabled.
        /// </summary>
        /// <value><b>true</b> if post comments can be searched, otherwise returns <b>false</b>.</value>
        public bool EnableCommentSearch
        {
            get => _enableCommentSearch;
            set => this.Set(ref _enableCommentSearch, value, Changed);
        }

        /// <summary>
        ///     If yes, checkbox for include comments in search added to UI
        /// </summary>
        public bool ShowIncludeCommentsOption
        {
            get => _showIncludeCommentsOption;
            set => this.Set(ref _showIncludeCommentsOption, value, Changed);
        }

        #endregion

        #region SearchButtonText

        /// <summary>
        ///     Gets or sets the search button text to be displayed.
        /// </summary>
        /// <value>The search button text to be displayed.</value>
        public string SearchButtonText
        {
            get => _searchButtonText;
            set => this.Set(ref _searchButtonText, value, Changed);
        }

        #endregion

        #region SearchCommentLabelText

        /// <summary>
        ///     Gets or sets the search comment label text to display.
        /// </summary>
        /// <value>The search comment label text to display.</value>
        public string SearchCommentLabelText
        {
            get => _searchCommentLabelText;
            set => this.Set(ref _searchCommentLabelText, value, Changed);
        }

        #endregion

        #region SearchDefaultText

        /// <summary>
        ///     Gets or sets the default search text to display.
        /// </summary>
        /// <value>The default search text to display.</value>
        public string SearchDefaultText
        {
            get => _searchDefaultText;
            set => this.Set(ref _searchDefaultText, value, Changed);
        }

        #endregion

        #region Endorsement

        /// <summary>
        ///     Gets or sets the URI of a web log that the author of this web log is promoting.
        /// </summary>
        /// <value>The <see cref = "Uri" /> of a web log that the author of this web log is promoting.</value>
        public string Endorsement
        {
            get => _endorsement;
            set => this.Set(ref _endorsement, value, Changed);
        }

        #endregion

        #region PostsPerFeed

        /// <summary>
        ///     Gets or sets the maximum number of characters that are displayed from a blog-roll retrieved post.
        /// </summary>
        /// <value>The maximum number of characters to display.</value>
        public int PostsPerFeed
        {
            get => _postsPerFeed;
            set => this.Set(ref _postsPerFeed, value, Changed);
        }

        #endregion

        #region AuthorName

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string AuthorName
        {
            get => _authorName;
            set => this.Set(ref _authorName, value, Changed);
        }

        #endregion

        #region Language

        /// <summary>
        ///     Gets or sets the language this blog is written in.
        /// </summary>
        /// <value>The language this blog is written in.</value>
        /// <remarks>
        ///     Recommended best practice for the values of the Language element is defined by RFC 1766 [RFC1766] which includes a two-letter Language Code (taken from the ISO 639 standard [ISO639]), 
        ///     followed optionally, by a two-letter Country Code (taken from the ISO 3166 standard [ISO3166]).
        /// </remarks>
        /// <example>
        ///     en-US
        /// </example>
        public string LanguageWrittenOn
        {
            get => _languageWrittenOn;
            set => this.Set(ref _languageWrittenOn, value, Changed);
        }

        #endregion

        #region GeocodingLatitude

        /// <summary>
        ///     Gets or sets the latitude component of the geocoding position for this blog.
        /// </summary>
        /// <value>The latitude value.</value>
        public float GeocodingLatitude
        {
            get => _geocodingLatitude;
            set => this.Set(ref _geocodingLatitude, value, Changed);
        }

        #endregion

        #region GeocodingLongitude

        /// <summary>
        ///     Gets or sets the longitude component of the geocoding position for this blog.
        /// </summary>
        /// <value>The longitude value.</value>
        public float GeocodingLongitude
        {
            get => _geocodingLongitude;
            set => this.Set(ref _geocodingLongitude, value, Changed);
        }

        #endregion

        #region ContactFormMessage

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string ContactFormMessage
        {
            get => _contactFormMessage;
            set => this.Set(ref _contactFormMessage, value, Changed);
        }

        #endregion

        #region ContactThankMessage

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string ContactThankMessage
        {
            get => _contactThankMessage;
            set => this.Set(ref _contactThankMessage, value, Changed);
        }

        #endregion

        #region ContactErrorMessage;
        /// <summary>
        ///     Gets or sets a custom error message for this blog.
        /// </summary>
        /// <value>The error messagge for this blog.</value>
        public string ContactErrorMessage
        {
            get => _contactErrorMessage;
            set => this.Set(ref _contactErrorMessage, value, Changed);
        }

        #endregion

        #region HtmlHeader

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string HtmlHeader
        {
            get => _htmlHeader;
            set => this.Set(ref _htmlHeader, value, Changed);
        }

        #endregion

        #region Culture

        /// <summary>
        ///     Gets or sets the name of the author of this blog.
        /// </summary>
        /// <value>The name of the author of this blog.</value>
        public string Culture
        {
            get => _culture;
            set => this.Set(ref _culture, value, Changed);
        }

        #endregion

        #region Timezone

        /// <summary>
        /// Time zone id
        /// </summary>
        public string TimeZoneId
        {
            get => _timeZoneId;
            set => this.Set(ref _timeZoneId, value, Changed);
        }

        /// <summary>
        /// Converts time saved to the storage as UTC 
        /// into local user time offset by timezone
        /// </summary>
        /// <param name="serverTime">FromUtc</param>
        /// <returns>Client time</returns>
        public DateTime FromUtc(DateTime? serverTime = null)
        {
            if (serverTime == null || serverTime == new DateTime())
                serverTime = DateTime.UtcNow;

            var zone = string.IsNullOrWhiteSpace(TimeZoneId) ? "UTC" : TimeZoneId;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(zone);
            serverTime = DateTime.SpecifyKind(serverTime.Value, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeFromUtc(serverTime.Value, tz);
        }

        /// <summary>
        /// Converts time passed from client into UTC time
        /// </summary>
        /// <param name="localTime">ToUtc</param>
        /// <returns>Server time</returns>
        public DateTime ToUtc(DateTime? localTime = null)
        {
            if (localTime == null || localTime == new DateTime()) // no time sent in, use "now"
                return DateTime.UtcNow;

            var zone = string.IsNullOrWhiteSpace(TimeZoneId) ? "UTC" : TimeZoneId;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(zone);
            localTime = DateTime.SpecifyKind(localTime.Value, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(localTime.Value, tz);
        }
        #endregion

        #region EnableContactAttachments

        /// <summary>
        ///     Gets or sets whether or not to allow visitors to send attachments via the contact form.
        /// </summary>
        public bool EnableContactAttachments
        {
            get => _enableContactAttachments;
            set => this.Set(ref _enableContactAttachments, value, Changed);
        }

        #endregion

        #region EnableRecaptchaOnContactForm

        /// <summary>
        ///     Gets or sets whether or not to use Recaptcha on the contact form.
        /// </summary>
        public bool EnableRecaptchaOnContactForm
        {
            get => _enableRecaptchaOnContactForm;
            set => this.Set(ref _enableRecaptchaOnContactForm, value, Changed);
        }

        #endregion

        #region RemoveExtensionsFromUrls

        /// <summary>
        ///     Gets or sets a value indicating if extensions (.aspx) should be removed from URLs
        ///     -- always returns true to prepare for transition to MVC style routing
        /// </summary>
        /// <value><b>true</b> if should be removed, otherwise returns <b>false</b>.</value>
        public bool RemoveExtensionsFromUrls { get { return true; } }

        #endregion

        #region RedirectToRemoveFileExtension

        /// <summary>
        ///     Gets or sets a value indicating if incoming requests containing extensions (.aspx) should be redirected to a URL with the extension removed.
        /// </summary>
        /// <value><b>true</b> if should be redirected, otherwise returns <b>false</b>.</value>
        public bool RedirectToRemoveFileExtension
        {
            get => _redirectToRemoveFileExtension;
            set => this.Set(ref _redirectToRemoveFileExtension, value, Changed);
        }

        #endregion

        /// <summary>
        /// Gets or sets whether this application's handlers should be able to download and cache files hosted on other servers.
        /// </summary>
        /// <remarks>
        /// 
        /// Allowing the server's various handlers(Such as JavaScriptHandler and CssHandler) could potentially allow a an attacker
        /// to tie up the server by continuously requesting files of excess file size, or from servers that take forever to time out.
        /// 
        /// This is false by default.
        /// 
        /// </remarks>
        public bool AllowServerToDownloadRemoteFiles
        {
            get => _allowServerToDownloadRemoteFiles;
            set => this.Set(ref _allowServerToDownloadRemoteFiles, value, Changed);
        }

        public string HeaderImage { get => LogoUrl; set => LogoUrl = value; }

        /// <summary>
        /// Dummy Id
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// [IMPORTANT] Last database migration date and time.
        /// </summary>
        public DateTime LastDatabaseMigration
        {
            get => _lastDatabaseMigration;
            set => this.Set(ref _lastDatabaseMigration, value, Changed);
        }

        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Is the website in maintenance mode?
        /// </summary>
        public bool MaintenanceMode
        {
            get => _maintenanceMode;
            set => this.Set(ref _maintenanceMode, value, Changed);
        }

        public bool New => false;

        /// <summary>
        /// Gets or sets the maximum length of time in milliseconds the server should spend downloading remote files. The default value is 30000.
        /// </summary>
        /// <remarks>
        /// 
        /// If the limit is set to something below 0, the defaultRemoteDownloadTimeout will be used instead.
        /// 0 is an acceptable value, users should use this value to indicate "unlimited time".
        /// </remarks>
        public int RemoteFileDownloadTimeout
        {
            get
            {
                if (_remoteDownloadTimeout < 0)
                {
                    _remoteDownloadTimeout = _defaultRemoteDownloadTimeout;
                }

                return _remoteDownloadTimeout;
            }
            set
            {
                if (value < 0) { value = _defaultRemoteDownloadTimeout; }
                this.Set(ref _remoteDownloadTimeout, value, Changed);
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed file size in bytes that BlogEngine can download from a remote server. Defaults to 512k.
        /// </summary>
        /// <remarks>
        /// 
        /// Set this value to 0 for unlimited file size.
        /// 
        /// </remarks>
        public int RemoteMaxFileSize
        {
            get
            {
                if (_maxRemoteFileSize < 0)
                {
                    _maxRemoteFileSize = _defaultMaxRemoteFileSize;
                }
                return _maxRemoteFileSize;
            }
            set
            {
                if (value < 0) { value = _defaultMaxRemoteFileSize; }
                this.Set(ref _maxRemoteFileSize, value, Changed);
            }
        }

        /// <summary>
        /// Rss Feed #1
        /// </summary>
        public string RssFeed1 { get; set; } = string.Empty;

        /// <summary>
        /// Rss Feed #2
        /// </summary>
        public string RssFeed2 { get; set; } = string.Empty;

        /// <summary>
        /// Rss Feed #3
        /// </summary>
        public string RssFeed3 { get; set; } = string.Empty;

        /// <summary>
        /// Rss Feed #4
        /// </summary>
        public string RssFeed4 { get; set; } = string.Empty;

        /// <summary>
        /// Rss Feed #5
        /// </summary>
        public string RssFeed5 { get; set; } = string.Empty;

        /// <summary>
        /// Extracts all settings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrigamiSetting> GetSettings()
        {
            var settings = new List<OrigamiSetting>();

            foreach (var property in GetType().GetProperties())
            {
                if (property.CanRead == false) continue;
                if (property.CanWrite == false) continue;
                if (property.Name.Like(nameof(Id)) == true) continue;
                if (property.Name.Like(nameof(SocialNetwork)) == true) continue;
                if (property.Name.Like(nameof(OpenTelemetry)) == true) continue;

                var name = property.Name.ToLower();
                var value = property.GetValue(this)?.ToString();
                var setting = new OrigamiSetting()
                {
                    Name = name,
                    Value = value ?? string.Empty,
                };
                settings.Add(setting);
            }

            settings.Add(this.OpenTelemetry);
            settings.Add(this.SocialNetwork);

            return settings;
        }

        #region Version()

        /// <summary>
        ///     The version.
        /// </summary>
        private string? _version;

        /// <summary>
        /// Returns the BlogEngine.NET version information.
        /// </summary>
        /// <value>
        /// The BlogEngine.NET major, minor, revision, and build numbers.
        /// </value>
        /// <remarks>
        /// The current version is determined by extracting the build version of the BlogEngine.Core assembly.
        /// </remarks>
        /// <returns>
        /// The version.
        /// </returns>
        public string Version()
        {
            return _version ?? (_version = GetType().Assembly.GetName().Version!.ToString());
        }

        #endregion

        #region "ErrorPage Title"
        /// <summary>
        ///     Gets or sets the Title Of Error Page.
        /// </summary>
        /// <value>The Title Error Page.</value>

        public string ErrorTitle
        {
            get => _errorTitle;
            set => this.Set(ref _errorTitle, value, Changed);
        }

        #endregion

        #region "ErrorPage Body"
        /// <summary>
        ///     Gets or sets the Body Of Error Page.
        /// </summary>
        /// <value>The Body Error Page.</value>
        public string ErrorText
        {
            get => _errorText;
            set => this.Set(ref _errorText, value, Changed);
        }


        #endregion

        #region EditorOptions

        public bool PageOptionsCustomFields
        {
            get => _pageOptionsCustomFields;
            set => this.Set(ref _pageOptionsCustomFields, value, Changed);
        }

        public bool PageOptionsDescription
        {
            get => _pageOptionsDescription;
            set => this.Set(ref _pageOptionsDescription, value, Changed);
        }

        public bool PageOptionsSlug
        {
            get => _pageOptionsSlug;
            set => this.Set(ref _pageOptionsSlug, value, Changed);
        }

        public bool PostOptionsCustomFields
        {
            get => _postOptionsCustomFields;
            set => this.Set(ref _postOptionsCustomFields, value, Changed);
        }

        public bool PostOptionsDescription
        {
            get => _postOptionsDescription;
            set => this.Set(ref _postOptionsDescription, value, Changed);
        }

        public bool PostOptionsSlug
        {
            get => _postOptionsSlug;
            set => this.Set(ref _postOptionsSlug, value, Changed);
        }

        #endregion

        #region SocialNetwork

        /// <summary>
        /// Social network settings
        /// </summary>
        public SocialNetwork SocialNetwork
        {
            get => _socialNetwork;
            set => this.Set(ref _socialNetwork, value, Changed);
        }

        #endregion

        #region OLTP

        /// <summary>
        /// OpenTelemetry settings
        /// </summary>
        public OpenTelemetry OpenTelemetry
        {
            get => _openTelemetry;
            set => this.Set(ref _openTelemetry, value, Changed);
        }

        #endregion
    }
}
