using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models.Events
{
    public class SocialProfileEditsCommentEvent : 
        OrigamiEvent, 
        IChanged,
        ICommentId
    {
        protected Guid _commentId;

        public SocialProfileEditsCommentEvent() : base()
        {
            this.Type = nameof(SocialProfileEditsCommentEvent);
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
