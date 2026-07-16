namespace Origami.Core.Models.Events
{
    public class UserDeletesCommentEvent : OrigamiEvent
    {
        public UserDeletesCommentEvent()
        {
            this.Type = nameof(UserDeletesCommentEvent);
        }
    }
}
