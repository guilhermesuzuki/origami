using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoRatings")]
    [Index(nameof(VideoId), nameof(SocialProfileId), IsUnique = true, Name = "IX_oi_VideoRatings_1")]
    public class OrigamiVideoRating :
        BaseView,
        IId,
        IFKVideo
    {
        private Guid _id;
        private Guid _videoId;

        private OrigamiVideo? _video;
        private byte _rating;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoRating() : base()
        {
            Id = Guid.NewGuid();
        }

        public event EventHandler<PropertyChangedEventArgs> RatingChanged = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, RatingChanged);
        }

        public Guid VideoId
        {
            get => _videoId;
            set => this.Set(ref _videoId, value, RatingChanged);
        }

        public OrigamiVideo? Video
        {
            get => _video;
            set => this.Set(ref _video, value, RatingChanged);
        }

        /// <summary>
        /// Rating for the Video
        /// </summary>
        public byte Rating
        {
            get => _rating;
            set => this.Set(ref _rating, value, RatingChanged);
        }
    }
}
