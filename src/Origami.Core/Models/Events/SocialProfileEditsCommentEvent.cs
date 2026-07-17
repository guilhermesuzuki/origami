using System.ComponentModel;

namespace Origami.Core.Models.Events
{
    public class SocialProfileEditsCommentEvent : 
        OrigamiEvent, 
        IChanged,
        ICommentId
    {
        protected Guid _commentId;

        public SocialProfileEditsCommentEvent()
        {
            this.Type = nameof(SocialProfileEditsCommentEvent);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public Guid CommentId
        {
            get => _commentId;
            set => this.Set(ref _commentId, value, Changed);
        }
    }
}
