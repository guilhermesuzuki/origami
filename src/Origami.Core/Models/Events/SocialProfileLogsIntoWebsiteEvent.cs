namespace Origami.Core.Models.Events
{
    public class SocialProfileLogsIntoWebsiteEvent : OrigamiEvent
    {
        public SocialProfileLogsIntoWebsiteEvent() : base()
        {
            this.Type = nameof(SocialProfileLogsIntoWebsiteEvent);
        }
    }
}
