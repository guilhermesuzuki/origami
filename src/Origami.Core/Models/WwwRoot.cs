namespace Origami.Core.Models
{
    public class WwwRoot : IWebRootPath
    {
        public WwwRoot(string wwwRootPath)
        {
            this.WebRootPath = Path.GetFullPath("..\\Origami.Files\\");
            this.WebRootPathForBackups = Path.Combine(this.WebRootPath, "files-backup");
        }

        public string WebRootPath { get; }
        public string WebRootPathForBackups { get; }
    }
}
