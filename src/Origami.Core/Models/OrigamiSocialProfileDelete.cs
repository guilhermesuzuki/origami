using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_SocialProfileDeleted")]
    [Index(nameof(DateCreated), nameof(SocialProfileId), IsUnique = true, Name = "IX_oi_SocialProfileDeleted_1")]
    public class OrigamiSocialProfileDelete :
        IId,
        IDateCreated,
        IFKSocialProfile,
        IChanged
    {
        private DateTime _dateCreated;
        private Guid _id = Guid.NewGuid();
        private int _postCommentReactions;
        private int _postComments;
        private int _postRatings;
        private OrigamiSocialProfile? _socialProfile;
        private Guid _socialProfileId;
        private int _videoCommentReactions;
        private int _videoComments;
        private int _videoRatings;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// Number of Post Comment Reactions Deleted
        /// </summary>
        public int PostCommentReactions
        {
            get => _postCommentReactions;
            set => this.Set(ref _postCommentReactions, value, Changed);
        }

        /// <summary>
        /// Number of Post Comments Deleted
        /// </summary>
        public int PostComments
        {
            get => _postComments;
            set => this.Set(ref _postComments, value, Changed);
        }

        /// <summary>
        /// Number of Post Ratings Deleted
        /// </summary>
        public int PostRatings
        {
            get => _postRatings;
            set => this.Set(ref _postRatings, value, Changed);
        }

        [ForeignKey(nameof(SocialProfileId))]
        public OrigamiSocialProfile? SocialProfile
        {
            get => _socialProfile;
            set => this.Set(ref _socialProfile, value, Changed);
        }

        public Guid SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, Changed);
        }

        /// <summary>
        /// Number of Video Reactions Deleted
        /// </summary>
        public int VideoCommentReactions
        {
            get => _videoCommentReactions;
            set => this.Set(ref _videoCommentReactions, value, Changed);
        }

        /// <summary>
        /// Number of Video Comments Deleted
        /// </summary>
        public int VideoComments
        {
            get => _videoComments;
            set => this.Set(ref _videoComments, value, Changed);
        }

        /// <summary>
        /// Number of Video Ratings Deleted
        /// </summary>
        public int VideoRatings
        {
            get => _videoRatings;
            set => this.Set(ref _videoRatings, value, Changed);
        }
    }
}
