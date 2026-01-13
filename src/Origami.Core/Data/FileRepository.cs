using Origami.Core.Models;
using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public class FileRepository :
        IFileRepository
    {
        protected readonly IWebRootPath _wwwRoot;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="wwwRoot"></param>
        public FileRepository(IWebRootPath wwwRoot)
            : base()
        {
            _wwwRoot = wwwRoot;
        }

        public bool FileExists(string virtualPath)
        {
            if (virtualPath.Has() == true)
            {
                var localpath = LocalPath(virtualPath);
                return new FileInfo(localpath).Exists;
            }

            return false;
        }

        public OrigamiSystemFile? GetFile(string virtualPath)
        {
            if (virtualPath.StartsWith("data:image") == true)
            {
                return null;
            }
            if (FileExists(virtualPath) == true)
            {
                var localpath = LocalPath(virtualPath);
                return new OrigamiSystemFile(localpath, virtualPath);
            }
            return GetJpg(virtualPath) ?? GetJpeg(virtualPath) ?? null;
        }

        public OrigamiSystemFile? GetJpeg(string virtualPath)
        {
            var oldValue = ".jpg";
            var newValue = ".jpeg";

            //path without the query string
            var path = virtualPath.Split('?')[0];

            if (path.EndsWith(oldValue, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                path = path.Replace(oldValue, newValue, true, null);

                if (FileExists(path) == true)
                {
                    var localpath = LocalPath(path);
                    return new OrigamiSystemFile(localpath, virtualPath);
                }
            }

            return null;
        }

        public OrigamiSystemFile? GetJpg(string virtualPath)
        {
            var oldValue = ".jpeg";
            var newValue = ".jpg";

            //path without the query string
            var path = virtualPath.Split('?')[0];

            if (path.EndsWith(oldValue, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                path = path.Replace(oldValue, newValue, true, null);

                if (FileExists(path) == true)
                {
                    var localpath = LocalPath(path);
                    return new OrigamiSystemFile(localpath, virtualPath);
                }
            }

            return null;
        }

        public string LocalPath(string virtualPath)
        {
            if (virtualPath.Has() == true)
            {
                virtualPath = virtualPath.TrimStart('~');
                virtualPath = virtualPath.TrimStart('/');
                virtualPath = virtualPath.Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(_wwwRoot.WebRootPath, virtualPath);
            }

            throw new ArgumentNullException(nameof(virtualPath));
        }
    }
}
