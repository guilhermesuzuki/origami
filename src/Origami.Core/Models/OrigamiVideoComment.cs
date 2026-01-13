using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoComments")]
    public class OrigamiVideoComment :
        BaseComment,
        ICommentChanged,
        IVideoId,
        IParentIdNull<OrigamiVideoComment>
    {
        protected Guid? _parentId;
        protected OrigamiVideo? _video;
        protected Guid _videoId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoComment() : base()
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

        public Guid VideoId
        {
            get => _videoId;
            set => this.Set(ref _videoId, value, CommentChanged);
        }
    }
}
