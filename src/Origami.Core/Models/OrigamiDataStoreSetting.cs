using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_DataStoreSettings")]
    public class OrigamiDataStoreSetting :
        IChanged,
        IId,
        IFKBlog
    {
        private Guid _id;
        private Guid _blogId;

        private OrigamiBlog? _blog;

        private string _extensionType = string.Empty;
        private string _extensionId = string.Empty;
        private string _settings = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiDataStoreSetting() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        [ForeignKey(nameof(BlogId))]
        public OrigamiBlog? Blog
        {
            get => _blog;
            set => this.Set(ref _blog, value, Changed);
        }

        [StringLength(50)]
        public string ExtensionType
        {
            get => _extensionType;
            set => this.Set(ref _extensionType, value, Changed);
        }

        [StringLength(100)]
        public string ExtensionId
        {
            get => _extensionId;
            set => this.Set(ref _extensionId, value, Changed);
        }

        /// <summary>
        /// Settings is NVARCHAR(MAX)
        /// </summary>
        public string Settings
        {
            get => _settings;
            set => this.Set(ref _settings, value, Changed);
        }
    }
}
