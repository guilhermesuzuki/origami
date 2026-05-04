namespace Origami.Core.Models
{
    public class HubContentSpecialMessage : HubContent<OrigamiSpecialMessage>
    {
        public HubContentSpecialMessage() : base()
        {
            this.Entity = new();
        }
    }
}
