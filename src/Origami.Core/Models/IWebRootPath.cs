namespace Origami.Core.Models
{
    /// <summary>
    /// Most basic interface to share the wwwroot directory full path.
    /// </summary>
    public interface IWebRootPath
    {
        /// <summary>
        /// wwwroot folder full path location
        /// </summary>
        string WebRootPath { get; }
    }
}
