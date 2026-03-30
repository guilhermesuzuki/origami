using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentCategories")]
    public class OrigamiContentCategory :
        IChanged,
        IId,
        ICategoryId,
        IContentId,
        IVersion,
        INew
    {
        protected Guid _categoryId;
        protected Guid _contentId;
        protected Guid _id = Guid.NewGuid();
        protected byte[] _version = Array.Empty<byte>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentCategory() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid CategoryId
        {
            get => _categoryId;
            set => this.Set(ref _categoryId, value, Changed);
        }

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public bool New => _version.SequenceEqual(Array.Empty<byte>());

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
