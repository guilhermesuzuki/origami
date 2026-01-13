using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// TODO: comment this
    /// </summary>
    public class VideoViewTotal :
        IChanged,
        IFKVideo
    {
        private Guid _videoId;
        private OrigamiVideo? _video;
        private long _totalViews;

        /// <summary>
        /// Default constructor
        /// </summary>
        public VideoViewTotal() : base()
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
        /// Total number of views
        /// </summary>
        public long TotalViews
        {
            get => _totalViews;
            set => this.Set(ref _totalViews, value, Changed);
        }
    }
}
