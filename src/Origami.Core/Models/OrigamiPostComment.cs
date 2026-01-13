using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PostComments")]
    public class OrigamiPostComment :
        BaseComment,
        ICommentChanged,
        IPostId,
        IParentIdNull<OrigamiPostComment>
    {
        protected Guid? _parentId;
        protected Guid _postId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPostComment() : base()
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

        public Guid PostId
        {
            get => _postId;
            set => this.Set(ref _postId, value, CommentChanged);
        }
    }
}
