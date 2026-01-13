using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    [Table("oi_Posts")]
    public class OrigamiPost :
        BaseContent,
        IBlogId,
        IContentChanged,
        IId,
        IAdditionalInfo<AdditionalInfo.ForPosts>,
        ILanguageWrittenOn,
        IHeaderImage
    {
        protected Guid _blogId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPost() : base()
        {
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiPost(Guid id) : this()
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
            set => Set(x => x.HeaderImage = value);
        }

        [Key]
        public override Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ContentChanged);
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

        /// <summary>
        /// Fake post
        /// </summary>
        public static OrigamiPost GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake posts
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiPost> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }

        public AdditionalInfo.ForPosts Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForPosts>();
        }

        public AdditionalInfo.ForPosts Set(Action<AdditionalInfo.ForPosts> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForPosts>();
        }
    }
}