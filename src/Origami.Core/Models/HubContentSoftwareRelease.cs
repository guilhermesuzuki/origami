namespace Origami.Core.Models
{
    public class HubContentSoftwareRelease : HubContent<OrigamiSoftwareRelease>
    {
        public HubContentSoftwareRelease() : base()
        {
            this.Entity = new();
        }
    }
}
