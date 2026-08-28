using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models.Events
{
    public class SocialProfilePinsCommentEvent :
        OrigamiEvent,
        IChanged,
        ICommentId
    {
        protected Guid _commentId;

        public SocialProfilePinsCommentEvent() : base()
        {
            this.Type = nameof(SocialProfilePinsCommentEvent);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        [Column(nameof(CommentId))]
        public Guid CommentId
        {
            get => _commentId;
            set => this.Set(ref _commentId, value, Changed);
        }
    }
}
