using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public interface IFileRepository
    {
        /// <summary>
        /// boolean whether a file exists by its virtual path
        /// </summary>
        /// <param name="virtualPath">the virtual path</param>
        /// <returns>boolean</returns>
        bool FileExists(string virtualPath);

        /// <summary>
        /// gets a specific file by virtual path
        /// </summary>
        /// <param name="virtualPath">the virtual path of the file</param>
        /// <returns></returns>
        OrigamiSystemFile? GetFile(string virtualPath);

        OrigamiSystemFile? GetJpeg(string virtualPath);

        OrigamiSystemFile? GetJpg(string virtualPath);

        /// <summary>
        /// returns the local path of a file or directory
        /// </summary>
        /// <param name="virtualPath">the virtual path of the file/directory</param>
        /// <returns></returns>
        string LocalPath(string virtualPath);
    }
}
