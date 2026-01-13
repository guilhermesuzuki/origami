using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PostRatings")]
    [Index(nameof(PostId), nameof(SocialProfileId), IsUnique = true, Name = "IX_oi_PostRatings_1")]
    public class OrigamiPostRating :
        BaseView,
        IRatingChanged,
        IId,
        IFKPost
    {
        private Guid _id;
        private Guid _postId;

        private OrigamiPost? _post;
        private byte _rating;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPostRating() : base()
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

        public Guid PostId
        {
            get => _postId;
            set => this.Set(ref _postId, value, RatingChanged);
        }

        public OrigamiPost? Post
        {
            get => _post;
            set => this.Set(ref _post, value, RatingChanged);
        }

        /// <summary>
        /// Rating for the Post
        /// </summary>
        public byte Rating
        {
            get => _rating;
            set => this.Set(ref _rating, value, RatingChanged);
        }
    }
}
