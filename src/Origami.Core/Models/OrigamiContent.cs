using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    [Table("oi_Contents")]
    public abstract class OrigamiContent :
        BaseContent,
        IBlogIdNull,
        IContentChanged,
        IId,
        IAdditionalInfo<AdditionalInfo.ForContents>,
        ILanguageWrittenOn,
        IHeaderImage,
        IParentIdNull,
        IType,
        ISubtypeNull
    {
        protected Guid? _parentId;
        protected string? _subtype;
        protected string _type = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContent() : base()
        {
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        public event EventHandler<PropertyChangedEventArgs> ContentChanged = delegate { };


        [NotMapped]
        public string HeaderImage
        {
            get => Get().HeaderImage;
            set => Set(x => x.HeaderImage = value);
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

        [StringLength(64)]
        public string? Subtype
        {
            get => _subtype;
            set => this.Set(ref _subtype, value, ContentChanged);
        }

        [StringLength(64)]
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, ContentChanged);
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