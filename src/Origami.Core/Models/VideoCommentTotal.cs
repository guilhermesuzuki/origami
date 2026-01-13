using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// TODO: comment this
    /// </summary>
    public class VideoCommentTotal :
        IChanged,
        IFKVideo
    {
        private Guid _videoId;
        private OrigamiVideo? _video;
        private long _totalComments;

        /// <summary>
        /// Default constructor
        /// </summary>
        public VideoCommentTotal() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid VideoId
        {
            get => _videoId;
            set => this.Set(ref _videoId, value, Changed);
        }

        [NotMapped]
        [ForeignKey(nameof(VideoId))]
        public OrigamiVideo? Video
        {
            get => _video;
            set => this.Set(ref _video, value, Changed);
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
