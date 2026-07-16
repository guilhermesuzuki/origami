namespace Origami.Core.Models.Events
{
    public class UserUnsubscribesFromWebsiteEvent : OrigamiEvent
    {
        public UserUnsubscribesFromWebsiteEvent()
        {
            this.Type = nameof(UserUnsubscribesFromWebsiteEvent);
        }
    }
}
