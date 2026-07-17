using System.ComponentModel;

namespace Origami.Core.Models.Events
{
    public class SocialProfileRepliesToCommentEvent : 
        OrigamiEvent
    {
        protected Guid _commentId;

        public SocialProfileRepliesToCommentEvent() : base()
        {
            this.Type = nameof(SocialProfileRepliesToCommentEvent);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public Guid CommentId
        {
            get => _commentId;
            set => this.Set(ref _commentId, value, Changed);
        }
    }
}
