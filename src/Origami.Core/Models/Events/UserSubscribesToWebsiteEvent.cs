namespace Origami.Core.Models.Events
{
    public class UserSubscribesToWebsiteEvent : OrigamiEvent
    {
        public UserSubscribesToWebsiteEvent()
        {
            this.Type = nameof(UserSubscribesToWebsiteEvent);
        }
    }
}
