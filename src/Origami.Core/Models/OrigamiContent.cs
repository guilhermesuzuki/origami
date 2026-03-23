using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    [Table("oi_Contents")]
    public class OrigamiContent :
        BaseContent,
        IBlogIdNull,
        IContentChanged,
        IId,
        IAdditionalInfo<AdditionalInfo.ForContents>,
        ILanguageWrittenOn,
        IHeaderImage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContent() : base()
        {
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiContent(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> ContentChanged = (sender, e) => { };

        

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

        public AdditionalInfo.ForContents Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForContents>();
        }

        public AdditionalInfo.ForContents Set(Action<AdditionalInfo.ForContents> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForContents>();
        }
    }
}