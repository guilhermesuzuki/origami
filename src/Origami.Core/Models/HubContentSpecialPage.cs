namespace Origami.Core.Models
{
    public class HubContentSpecialPage : HubContent<OrigamiSpecialPage>
    {
        public HubContentSpecialPage() : base()
        {
            this.Entity = new();
        }
    }
}
