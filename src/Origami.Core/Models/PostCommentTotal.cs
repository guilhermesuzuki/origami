using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// Class to represent total post comments with Dapper
    /// </summary>
    public class PostCommentTotal :
        IChanged,
        IFKPost
    {
        private OrigamiPost? _post;
        private Guid _postId;
        private long _totalComments;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PostCommentTotal() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [NotMapped]
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

        /// <summary>
        /// Total number of comments
        /// </summary>
        public long TotalComments
        {
            get => _totalComments;
            set => this.Set(ref _totalComments, value, Changed);
        }
    }
}
