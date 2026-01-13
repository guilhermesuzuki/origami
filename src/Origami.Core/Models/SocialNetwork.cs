using Origami.Core.Models.Settings;

namespace Origami.Core.Models
{
    public class SocialNetwork
    {
        public Facebook Facebook { get; set; } = new();
        public GitHub GitHub { get; set; } = new();
        public Google Google { get; set; } = new();
        public Settings.Microsoft Microsoft { get; set; } = new();
        public Twitter Twitter { get; set; } = new();
    }
}
