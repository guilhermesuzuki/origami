using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    [Table("oi_Pages")]
    public class OrigamiPage :
        BaseContent,
        IBlogId,
        IContentChanged,
        IId,
        IParentIdNull<OrigamiPage>,
        IAdditionalInfo<AdditionalInfo.ForPages>,
        ILanguageWrittenOn,
        IHeaderImage
    {
        protected Guid _blogId;
        protected bool _isFrontPage;
        protected string? _keywords;
        protected Guid? _parentId;
        protected bool _showInList;
        protected int _sortOrder;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPage() : base()
        {
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiPage(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> ContentChanged = (sender, e) => { };

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, ContentChanged);
        }

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
        /// Is this Page the Front Page?
        /// </summary>
        public bool IsFrontPage
        {
            get => _isFrontPage;
            set => this.Set(ref _isFrontPage, value, ContentChanged);
        }

        /// <summary>
        /// Page Keywords (nvarchar[max])
        /// </summary>
        public string? Keywords
        {
            get => _keywords;
            set => this.Set(ref _keywords, value, ContentChanged);
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

        public Guid? ParentId
        {
            get => _parentId;
            set => this.Set(ref _parentId, value, ContentChanged);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this page should be in the sitemap list.
        /// </summary>
        public bool ShowInList
        {
            get => _showInList;
            set => this.Set(ref _showInList, value, ContentChanged);
        }

        /// <summary>
        /// Sort Order
        /// </summary>
        public int SortOrder
        {
            get => _sortOrder;
            set => this.Set(ref _sortOrder, value, ContentChanged);
        }

        /// <summary>
        /// Fake page
        /// </summary>
        public static OrigamiPage GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake pages
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiPage> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }

        public AdditionalInfo.ForPages Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForPages>();
        }

        public AdditionalInfo.ForPages Set(Action<AdditionalInfo.ForPages> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForPages>();
        }
    }
}
