namespace Origami.Core.Models.Events
{
    public class SocialProfileSubscribesToWebsiteEvent : OrigamiEvent
    {
        public SocialProfileSubscribesToWebsiteEvent() : base()
        {
            this.Type = nameof(SocialProfileSubscribesToWebsiteEvent);
        }
    }
}
