using Origami.Core.Models;
using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public interface IDirectoryRepository
    {
        /// <summary>
        /// creates a directory given a virtual path in the local file system
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool Create(string virtualPath);

        /// <summary>
        /// boolean whether a directory exists by its virtual path
        /// </summary>
        /// <param name="virtualPath">the virtual path</param>
        /// <returns>boolean</returns>
        bool DirectoryExists(string virtualPath);

        /// <summary>
        /// gets a specific directory by virtual path
        /// </summary>
        /// <param name="virtualPath">the virtual path of the directory</param>
        /// <returns></returns>
        OrigamiSystemDirectory GetDirectory(string virtualPath, bool create = true);

        /// <summary>
        /// gets all files from a virtual path
        /// </summary>
        /// <param name="virtualPath">the virtual path of the directory</param>
        /// <returns></returns>
        IEnumerable<OrigamiSystemFile> GetFiles(string virtualPath);

        /// <summary>
        /// returns the local path of a directory
        /// </summary>
        /// <param name="virtualPath">the virtual path of the directory</param>
        /// <returns></returns>
        string LocalPath(string virtualPath);

        /// <summary>
        /// Returns the local path for files (eg C:\inetpub\wwwroot\files\blogs\27604F05-86AD-47EF-9E05-950BB762570C\{plural of T}\<paramref name="entity"/>.Id\)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        string LocalPathForFiles<T>(T entity) where T : IId;

        /// <summary>
        /// returns the web path for files (eg /files/blogs/27604F05-86AD-47EF-9E05-950BB762570C/{plural of T}/<paramref name="entity"/>.Id/)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        string WebPathForFiles<T>(T entity) where T : IId;
    }
}
