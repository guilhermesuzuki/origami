using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// Class to represent total post views with Dapper
    /// </summary>
    public class PostViewTotal :
        IChanged,
        IFKPost
    {
        private OrigamiPost? _post;
        private Guid _postId;
        private long _totalViews;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PostViewTotal() : base()
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
        /// Total number of views
        /// </summary>
        public long TotalViews
        {
            get => _totalViews;
            set => this.Set(ref _totalViews, value, Changed);
        }
    }
}
