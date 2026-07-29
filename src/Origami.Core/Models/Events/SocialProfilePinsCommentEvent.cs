using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
        public Guid CommentId
        {
            get => _commentId;
            set => this.Set(ref _commentId, value, Changed);
        }
    }
}
