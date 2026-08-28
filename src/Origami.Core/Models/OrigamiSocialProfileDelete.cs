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
        IChanged
    {
        private int _contentCommentReactions;
        private int _contentComments;
        private int _contentRatings;
        private int _contentReactions;
        private DateTime _dateCreated;
        private Guid _id = Guid.NewGuid();
        private OrigamiSocialProfile? _socialProfile;
        private Guid _socialProfileId;
        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Number of Content Comment Reactions Deleted
        /// </summary>
        public int ContentCommentReactions
        {
            get => _contentCommentReactions;
            set => this.Set(ref _contentCommentReactions, value, Changed);
        }

        /// <summary>
        /// Number of Content Comments Deleted
        /// </summary>
        public int ContentComments
        {
            get => _contentComments;
            set => this.Set(ref _contentComments, value, Changed);
        }

        /// <summary>
        /// Number of Content Ratings Deleted
        /// </summary>
        public int ContentRatings
        {
            get => _contentRatings;
            set => this.Set(ref _contentRatings, value, Changed);
        }

        /// <summary>
        /// Number of Content Reactions Deleted
        /// </summary>
        public int ContentReactions
        {
            get => _contentReactions;
            set => this.Set(ref _contentReactions, value, Changed);
        }

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

        public Guid SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, Changed);
        }
    }
}
