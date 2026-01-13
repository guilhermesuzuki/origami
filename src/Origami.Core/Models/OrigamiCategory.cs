using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Categories")]
    public class OrigamiCategory :
        BaseModel,
        IModel,
        IChanged,
        IBlogId,
        IParentIdNull<OrigamiCategory>,
        IName,
        IDescriptionNull,
        IAdditionalInfo,
        IAdditionalInfo<AdditionalInfo.ForCategories>,
        IHeaderImage,
        IDeleted,
        IDateCreated,
        IDateModified,
        ISlug
    {
        protected string? _additionalInfo = string.Empty;
        protected Guid _blogId;
        protected DateTime _dateCreated;
        protected DateTime? _dateModified;
        protected string? _description = string.Empty;
        protected bool _isDeleted;
        protected string _name = string.Empty;
        protected Guid? _parentId;
        protected byte[] _version = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiCategory() : base()
        {
            this.NanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 6);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public string? AdditionalInfo
        {
            get => _additionalInfo;
            set => this.Set(ref _additionalInfo, value, Changed);
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        /// <summary>
        /// Date/Time this Content was Created
        /// </summary>
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        /// <summary>
        /// Date/Time this Page was Modified
        /// </summary>
        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        /// <summary>
        /// Category description
        /// </summary>
        [StringLength(200)]
        public string? Description
        {
            get => _description;
            set => this.Set(ref _description, value, Changed);
        }

        [NotMapped]
        public string HeaderImage
        {
            get => Get().Image.Source;
            set => Set(x => x.Image.Source = value);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        /// <summary>
        /// Category name
        /// </summary>
        [StringLength(50)]
        public string Name
        {
            get => _name;
            set => this.Set(ref _name, value, Changed);
        }

        public bool New => Version.SequenceEqual([]);

        /// <summary>
        /// Parent Category Id (FK)
        /// </summary>
        public Guid? ParentId
        {
            get => _parentId;
            set => this.Set(ref _parentId, value, Changed);
        }

        [NotMapped]
        public string Slug => Name.GetSlug();

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }

        public AdditionalInfo.ForCategories Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForCategories>();
        }

        public AdditionalInfo.ForCategories Set(Action<AdditionalInfo.ForCategories> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForCategories>();
        }
    }
}
