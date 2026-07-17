namespace Origami.Core.Models.Events
{
    public class SocialProfileRepliesToContentEvent : OrigamiEvent
    {
        public SocialProfileRepliesToContentEvent() : base()
        {
            this.Type = nameof(SocialProfileRepliesToContentEvent);
        }
    }
}
