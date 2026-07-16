namespace Origami.Core.Models.Events
{
    public class UserLogsIntoWebsiteEvent : OrigamiEvent
    {
        public UserLogsIntoWebsiteEvent()
        {
            this.Type = nameof(UserLogsIntoWebsiteEvent);
        }
    }
}
