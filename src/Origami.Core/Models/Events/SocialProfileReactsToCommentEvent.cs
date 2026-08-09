using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models.Events
{
    public class SocialProfileReactsToCommentEvent :
        OrigamiEvent,
        IChanged,
        IReactionId
    {
        protected Guid _reactionId;

        public SocialProfileReactsToCommentEvent() : base()
        {
            this.Type = nameof(SocialProfileReactsToCommentEvent);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        [Column("CommentReactionId")]
        public Guid ReactionId
        {
            get => _reactionId;
            set => this.Set(ref _reactionId, value, Changed);
        }
    }
}
