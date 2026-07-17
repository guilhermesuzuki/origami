namespace Origami.Core.Models.Events
{
    public class SocialProfileUnsubscribesFromWebsiteEvent : OrigamiEvent
    {
        public SocialProfileUnsubscribesFromWebsiteEvent()
        {
            this.Type = nameof(SocialProfileUnsubscribesFromWebsiteEvent);
        }
    }
}
