namespace Origami.Core.Models
{
    public class HubContentPost : HubContent<OrigamiPost>
    {
        public HubContentPost() : base()
        {
            this.Entity = new();
        }
    }
}
