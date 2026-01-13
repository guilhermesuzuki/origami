using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    [Table("oi_SpecialPages")]
    public class OrigamiSpecialPage :
        BaseContent,
        IContentChanged,
        IId,
        IAdditionalInfo<AdditionalInfo.ForSitePages>,
        ILanguageWrittenOn,
        IHeaderImage,
        IType
    {
        /// <summary>
        /// Default maintenance page
        /// </summary>
        public readonly static OrigamiSpecialPage Maintenance = new()
        {
            Id = Guid.Parse("7B66B400-69CC-4974-BE2F-BDA3F45DD38C"),
            Content = "This website is under maintenance.",
            LanguageWrittenOn = "en-US",
            Title = "Maintenance page",
            Type = OrigamiSpecialPageTypes.Maintenance.ToString(),
        };

        protected string _type = OrigamiSpecialPageTypes.CookiePolicy.ToString();

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSpecialPage() : base()
        {
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiSpecialPage(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> ContentChanged = (sender, e) => { };
        [NotMapped]
        public string HeaderImage
        {
            get => Get().HeaderImage;
            set => Set(info => info.HeaderImage = value);
        }

        [Key]
        public override Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ContentChanged);
        }

        /// <summary>
        /// This is not mapped for Pages (Pages don't have comments)
        /// </summary>
        [NotMapped]
        public override bool IsCommentEnabled
        {
            get => _isCommentEnabled;
            set => this.Set(ref _isCommentEnabled, value, ContentChanged);
        }
        /// <summary>
        /// Language this page was written on
        /// </summary>
        [NotMapped]
        public string LanguageWrittenOn
        {
            get => Get().LanguageWrittenOn;
            set => Set(x => x.LanguageWrittenOn = value);
        }

        [StringLength(25)]
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, ContentChanged);
        }

        /// <summary>
        /// Fake page
        /// </summary>
        public static OrigamiSpecialPage GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake pages
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiSpecialPage> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }

        public AdditionalInfo.ForSitePages Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForSitePages>();
        }

        public AdditionalInfo.ForSitePages Set(Action<AdditionalInfo.ForSitePages> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForSitePages>();
        }
    }
}
