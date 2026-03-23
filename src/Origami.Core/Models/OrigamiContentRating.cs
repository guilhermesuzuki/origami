using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentRatings")]
    public class OrigamiContentRating :
        BaseView,
        IRatingChanged,
        IId,
        IContentId
    {
        private Guid _id;
        private Guid _contentId;
        private byte _rating;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentRating() : base()
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

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, RatingChanged);
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
