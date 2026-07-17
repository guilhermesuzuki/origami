using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models.Events
{
    public class SocialProfileReactsToContentEvent : 
        OrigamiEvent, 
        IChanged,
        IReactionId
    {
        protected Guid _reactionId;

        public SocialProfileReactsToContentEvent()
        {
            this.Type = nameof(SocialProfileReactsToContentEvent);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        [Column("ContentReactionId")]
        public Guid ReactionId
        {
            get => _reactionId;
            set => this.Set(ref _reactionId, value, Changed);
        }
    }
}
