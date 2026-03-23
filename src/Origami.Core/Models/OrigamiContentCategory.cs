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
        IContentId
    {
        protected Guid _categoryId;
        protected Guid _id = Guid.NewGuid();
        protected Guid _contentId;

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

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
        }
    }
}
