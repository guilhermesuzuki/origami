namespace Origami.Core.Models
{
    public class WwwRoot : IWebRootPath
    {
        public WwwRoot(string wwwRootPath)
        {
            WebRootPath = wwwRootPath;
        }

        public string WebRootPath { get; init; }
    }
}
