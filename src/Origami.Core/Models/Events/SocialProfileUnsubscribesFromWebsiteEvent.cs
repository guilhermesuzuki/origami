namespace Origami.Core.Models.Events
{
    public class SocialProfileUnsubscribesFromWebsiteEvent : OrigamiEvent
    {
        public SocialProfileUnsubscribesFromWebsiteEvent() : base()
        {
            this.Type = nameof(SocialProfileUnsubscribesFromWebsiteEvent);
        }
    }
}
