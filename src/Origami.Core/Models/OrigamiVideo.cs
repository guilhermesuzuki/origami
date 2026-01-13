using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    [Table("oi_Videos")]
    public class OrigamiVideo :
        BaseContent,
        IBlogId,
        IContentChanged,
        IId,
        IAdditionalInfo<AdditionalInfo.ForVideos>,
        ILanguageWrittenOn,
        IHeaderImage,
        IDateReleased,
        IVideo,
        IEmbedIFrame
    {
        protected Guid _blogId;
        protected DateTime? _dateReleased;
        protected OrigamiFile _mediaFile = new();
        protected OrigamiFile? _subtitle1;
        protected OrigamiFile? _subtitle2;
        protected OrigamiFile? _subtitle3;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideo() : base()
        {
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiVideo(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> ContentChanged = (sender, e) => { };

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, ContentChanged);
        }

        /// <summary>
        /// Date/Time this Content was Created
        /// </summary>
        public DateTime? DateReleased
        {
            get => _dateReleased;
            set => this.Set(ref _dateReleased, value, ContentChanged);
        }

        [NotMapped]
        public string HeaderImage
        {
            get => Get().HeaderImage;
            set => Set(x => x.HeaderImage = value);
        }

        [Key]
        public override Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ContentChanged);
        }

        /// <summary>
        /// Language this video was written on
        /// </summary>
        [NotMapped]
        public string LanguageWrittenOn
        {
            get => Get().LanguageWrittenOn;
            set => Set(x => x.LanguageWrittenOn = value);
        }

        [NotMapped]
        public string EmbedIFrame
        {
            get => Get().EmbedIFrame;
            set => Set(x => x.EmbedIFrame = value);
        }

        /// <summary>
        /// Media File for this Video
        /// </summary>
        public OrigamiFile MediaFile
        {
            get => _mediaFile;
            set => this.Set(ref _mediaFile, value, ContentChanged);
        }

        /// <summary>
        /// Subtitle 1
        /// </summary>
        public OrigamiFile? Subtitle1
        {
            get => _subtitle1;
            set => this.Set(ref _subtitle1, value, ContentChanged);
        }

        /// <summary>
        /// Subtitle 2
        /// </summary>
        public OrigamiFile? Subtitle2
        {
            get => _subtitle2;
            set => this.Set(ref _subtitle2, value, ContentChanged);
        }

        /// <summary>
        /// Subtitle 3
        /// </summary>
        public OrigamiFile? Subtitle3
        {
            get => _subtitle3;
            set => this.Set(ref _subtitle3, value, ContentChanged);
        }

        /// <summary>
        /// Fake video
        /// </summary>
        public static OrigamiVideo GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake videos
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiVideo> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }
        public AdditionalInfo.ForVideos Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForVideos>();
        }

        /// <summary>
        /// Extracts all subtitles in this video
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrigamiFile> GetSubtitles()
        {
            var result = new List<OrigamiFile>();

            if (Subtitle1 != null) result.Add(Subtitle1);
            if (Subtitle2 != null) result.Add(Subtitle2);
            if (Subtitle3 != null) result.Add(Subtitle3);

            return result;
        }

        public AdditionalInfo.ForVideos Set(Action<AdditionalInfo.ForVideos> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForVideos>();
        }
    }
}