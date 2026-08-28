using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentComments")]
    public class OrigamiContentComment :
        BaseComment,
        ICommentChanged,
        IContentId,
        IParentIdNull
    {
        protected Guid? _parentId;
        protected Guid _contentId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentComment() : base()
        {
            Id = Guid.NewGuid();
        }

        public event EventHandler<PropertyChangedEventArgs> CommentChanged = (sender, e) => { };

        /// <summary>
        /// Parent Comment Id (In Reply To)
        /// </summary>
        public Guid? ParentId
        {
            get => _parentId;
            set => this.Set(ref _parentId, value, CommentChanged);
        }

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, CommentChanged);
        }
    }
}
