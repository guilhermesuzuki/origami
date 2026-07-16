namespace Origami.Core.Models.Events
{
    public class UserReactsToCommentEvent : OrigamiEvent
    {
        public UserReactsToCommentEvent()
        {
            this.Type = nameof(UserReactsToCommentEvent);
        }
    }
}
