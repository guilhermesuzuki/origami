using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Origami.Core.Models.Events
{
    public class SocialProfileUnpinsCommentEvent :
        OrigamiEvent,
        IChanged,
        ICommentId
    {
        protected Guid _commentId;

        public SocialProfileUnpinsCommentEvent() : base()
        {
            this.Type = nameof(SocialProfileUnpinsCommentEvent);
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
