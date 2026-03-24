using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// Class to represent total post comments with Dapper
    /// </summary>
    public class PostCommentTotal :
        IChanged,
        IPostId
    {
        private Guid _postId;
        private long _totalComments;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PostCommentTotal() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

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
