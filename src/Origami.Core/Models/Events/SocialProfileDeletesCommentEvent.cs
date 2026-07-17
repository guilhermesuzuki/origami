namespace Origami.Core.Models.Events
{
    public class SocialProfileDeletesCommentEvent : OrigamiEvent
    {
        public SocialProfileDeletesCommentEvent()
        {
            this.Type = nameof(SocialProfileDeletesCommentEvent);
        }
    }
}
