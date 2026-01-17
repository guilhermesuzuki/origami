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

        /// <summary>
        /// Gets the file system path to the web root directory used for storing backup files.
        /// </summary>
        string WebRootPathForBackups { get; }

        /// <summary>
        /// Gets the file system path to the web root directory used for storing restore files.
        /// </summary>
        string WebRootPathForRestores { get; }
    }
}
