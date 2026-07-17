namespace Origami.Core.Models.Events
{
    public class SocialProfileReactsToContentEvent : OrigamiEvent
    {
        public SocialProfileReactsToContentEvent()
        {
            this.Type = nameof(SocialProfileReactsToContentEvent);
        }
    }
}
