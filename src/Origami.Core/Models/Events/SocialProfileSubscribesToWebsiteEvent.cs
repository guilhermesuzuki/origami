namespace Origami.Core.Models.Events
{
    public class SocialProfileSubscribesToWebsiteEvent : OrigamiEvent
    {
        public SocialProfileSubscribesToWebsiteEvent()
        {
            this.Type = nameof(SocialProfileSubscribesToWebsiteEvent);
        }
    }
}
