namespace Origami.Core.Models.Events
{
    public class SocialProfileRepliesToCommentEvent : OrigamiEvent
    {
        public SocialProfileRepliesToCommentEvent()
        {
            this.Type = nameof(SocialProfileRepliesToCommentEvent);
        }
    }
}
