namespace Origami.Core.Models.Events
{
    public class SocialProfileLogsIntoWebsiteEvent : OrigamiEvent
    {
        public SocialProfileLogsIntoWebsiteEvent()
        {
            this.Type = nameof(SocialProfileLogsIntoWebsiteEvent);
        }
    }
}
