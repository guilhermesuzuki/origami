namespace Origami.Core.Models.Events
{
    public class SocialProfileReactsToCommentEvent : OrigamiEvent
    {
        public SocialProfileReactsToCommentEvent()
        {
            this.Type = nameof(SocialProfileReactsToCommentEvent);
        }
    }
}
