using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Blogs")]
    public class OrigamiBlog :
        BaseModel,
        IModel,
        IChanged,
        IName,
        IDeleted,
        IDateCreated,
        IDateModified,
        IAdditionalInfo,
        IAdditionalInfo<AdditionalInfo.ForBlogs>,
        IHeaderImage,
        IHyperlink,
        ISlug
    {
        public static readonly OrigamiBlog Empty = new() { Id = Guid.Empty };

        protected string? _additionalInfo;
        protected DateTime _dateCreated;
        protected DateTime? _dateModified;
        protected bool _isActive;
        protected bool _isDeleted;
        protected bool _isPrimary;
        protected bool _isSelected;
        protected string _name = string.Empty;
        protected byte[] _version = [];

        protected string _slug = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiBlog() : base()
        {
            
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => 
        { 
            if (sender is OrigamiBlog blog)
            {
                if (e.PropertyName == nameof(Name))
                {
                    blog.Slug = blog.Name.GetSlug();
                }
            }
        };

        public string? AdditionalInfo
        {
            get => _additionalInfo;
            set => this.Set(ref _additionalInfo, value, Changed);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        [NotMapped]
        public string HeaderImage
        {
            get => Get().HeaderImage;
            set => Set(info => info.HeaderImage = value);
        }

        public string Hyperlink => $"/blogs/{Slug}/";

        /// <summary>
        /// Gets whether the blog instance is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => this.Set(ref _isActive, value, Changed);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        /// <summary>
        /// Gets whether the blog is the primary blog instance.
        /// </summary>
        public bool IsPrimary
        {
            get => _isPrimary;
            set => this.Set(ref _isPrimary, value, Changed);
        }

        [NotMapped]
        public bool IsSelected
        {
            get => _isSelected;
            set => this.Set(ref _isSelected, value, Changed);
        }

        /// <summary>
        /// Blog Name
        /// </summary>
        [StringLength(255)]
        public string Name
        {
            get => _name;
            set => this.Set(ref _name, value, Changed);
        }

        public bool New => Version.SequenceEqual([]);

        [NotMapped]
        public int? Order
        {
            get => Get().Order;
            set => Set(info => info.Order = value);
        }

        [StringLength(255)]
        public string Slug 
        {
            get => _slug;
            set => this.Set(ref _slug, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }

        public AdditionalInfo.ForBlogs Get()
        {
            return AdditionalInfo.To<AdditionalInfo.ForBlogs>();
        }

        public AdditionalInfo.ForBlogs Set(Action<AdditionalInfo.ForBlogs> action)
        {
            AdditionalInfo = AdditionalInfo.From(action);
            return AdditionalInfo.To<AdditionalInfo.ForBlogs>();
        }
    }
}
