namespace Origami.Core.Models.Events
{
    public class UserEditsCommentEvent : OrigamiEvent
    {
        public UserEditsCommentEvent()
        {
            this.Type = nameof(UserEditsCommentEvent);
        }
    }
}
