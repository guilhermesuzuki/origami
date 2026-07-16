namespace Origami.Core.Models.Events
{
    public class UserRepliesToCommentEvent : OrigamiEvent
    {
        public UserRepliesToCommentEvent()
        {
            this.Type = nameof(UserRepliesToCommentEvent);
        }
    }
}
