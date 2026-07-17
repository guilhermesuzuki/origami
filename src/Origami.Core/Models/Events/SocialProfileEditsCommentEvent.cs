namespace Origami.Core.Models.Events
{
    public class SocialProfileEditsCommentEvent : OrigamiEvent
    {
        public SocialProfileEditsCommentEvent()
        {
            this.Type = nameof(SocialProfileEditsCommentEvent);
        }
    }
}
