using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoCommentReactions")]
    [Index(nameof(CommentId), nameof(SocialProfileId), nameof(Reaction), IsUnique = true, Name = "IX_oi_VideoCommentReactions_1")]
    public class OrigamiVideoCommentReaction :
        Reaction,
        IReactionChanged,
        IId,
        IFKComment<OrigamiVideoComment>
    {
        private OrigamiVideoComment? _comment;
        private Guid _commentId = Guid.Empty;
        private Guid _id = Guid.Empty;
        private string _reaction = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoCommentReaction() : base()
        {
            Id = Guid.NewGuid();
        }

        public event EventHandler<PropertyChangedEventArgs> ReactionChanged = (sender, e) => { };

        [ForeignKey(nameof(CommentId))]
        public OrigamiVideoComment? Comment
        {
            get => _comment;
            set => this.Set(ref _comment, value, ReactionChanged);
        }

        public Guid CommentId
        {
            get => _commentId;
            set => this.Set(ref _commentId, value, ReactionChanged);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ReactionChanged);
        }

        /// <summary>
        /// Reaction to this Comment
        /// </summary>
        [StringLength(5)]
        public string Reaction
        {
            get => _reaction;
            set => this.Set(ref _reaction, value, ReactionChanged);
        }
    }
}
