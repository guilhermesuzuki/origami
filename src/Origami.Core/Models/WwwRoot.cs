namespace Origami.Core.Models
{
    public class WwwRoot : IWebRootPath
    {
        public WwwRoot(string wwwRootPath)
        {
            this.WebRootPath = Path.GetFullPath("..\\Origami.Files\\");
        }

        public string WebRootPath { get; init; }
    }
}
