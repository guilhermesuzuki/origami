namespace Origami.Core.Models.Events
{
    public class UserReactsToContentEvent : OrigamiEvent
    {
        public UserReactsToContentEvent()
        {
            this.Type = nameof(UserReactsToContentEvent);
        }
    }
}
