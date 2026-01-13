using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PostCategories")]
    public class OrigamiPostCategory :
        IChanged,
        IId,
        ICategoryId,
        IFKPost
    {
        protected Guid _categoryId;
        protected Guid _id = Guid.NewGuid();
        protected OrigamiPost? _post;
        protected Guid _postId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPostCategory() : base()
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

        [ForeignKey(nameof(PostId))]
        public OrigamiPost? Post
        {
            get => _post;
            set => this.Set(ref _post, value, Changed);
        }

        public Guid PostId
        {
            get => _postId;
            set => this.Set(ref _postId, value, Changed);
        }
    }
}
