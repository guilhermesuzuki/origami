namespace Origami.Core.Models.Events
{
    public class UserRepliesToContentEvent : OrigamiEvent
    {
        public UserRepliesToContentEvent() : base()
        {
            this.Type = nameof(UserRepliesToContentEvent);
        }
    }
}
