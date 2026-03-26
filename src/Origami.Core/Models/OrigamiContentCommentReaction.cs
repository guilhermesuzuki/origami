using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentCommentReactions")]
    public class OrigamiContentCommentReaction :
        BaseTracking,
        IReactionChanged,
        IId,
        ICommentId
    {
        private Guid _commentId = Guid.Empty;
        private Guid _id = Guid.Empty;
        private string _reaction = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentCommentReaction() : base()
        {
            Id = Guid.NewGuid();
        }

        public event EventHandler<PropertyChangedEventArgs> ReactionChanged = (sender, e) => { };

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
