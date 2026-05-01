namespace Origami.Core.Models
{
    public class HubContentPage : HubContent<OrigamiPage>
    {
        public HubContentPage() : base()
        {
            this.Entity = new();
        }
    }
}
