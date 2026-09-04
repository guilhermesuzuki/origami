using Origami.Core.Models;
using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public class DirectoryRepository :
        IDirectoryRepository
    {
        protected readonly IMyMemoryCache _memoryCache;
        protected readonly IWebRootPath _wwwRoot;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="wwwRoot"></param>
        public DirectoryRepository(
            IMyMemoryCache myMemoryCache,
            IWebRootPath wwwRoot)
            : base()
        {
            _memoryCache = myMemoryCache;
            _wwwRoot = wwwRoot;
        }

        public bool Create(string virtualPath)
        {
            try
            {
                if (DirectoryExists(virtualPath) == false)
                {
                    var localpath = LocalPath(virtualPath);
                    new DirectoryInfo(localpath).Create();
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public bool DirectoryExists(string virtualPath)
        {
            var localpath = LocalPath(virtualPath);
            return new DirectoryInfo(localpath).Exists;
        }

        public OrigamiSystemDirectory GetDirectory(string virtualPath, bool create = true)
        {
            if (create) Create(virtualPath);

            if (DirectoryExists(virtualPath) == true)
            {
                var localpath = LocalPath(virtualPath);
                return new OrigamiSystemDirectory(localpath, virtualPath);
            }

            throw new DirectoryNotFoundException($"Directory {virtualPath} not found");
        }

        public IEnumerable<OrigamiSystemFile> GetFiles(string virtualPath)
        {
            if (DirectoryExists(virtualPath) == true)
            {
                return GetDirectory(virtualPath).Files;
            }

            throw new DirectoryNotFoundException($"Directory {virtualPath} not found");
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

        public string LocalPathForFiles<T>(T entity) where T : IId
        {
            var webPath = WebPathForFiles(entity);
            return LocalPath(webPath);
        }

        public bool Remove(OrigamiSystemFile file)
        {
            if (file == null) return false;

            if (File.Exists(file.LocalPath) == true)
            {
                File.Delete(file.LocalPath);
                return true;
            }

            return false;
        }

        public string WebPathForFiles<T>(T entity) where T : IId
        {
            if (entity is OrigamiSettings)
            {
                return $"/files/blogs/settings/";
            }

            if (entity is OrigamiBackup)
            {
                return $"/files-backup/";
            }

            var plural = typeof(T).GetPlural().Replace(' ', '-').ToLower();
            var directory = entity is INanoId nanoId ? nanoId.NanoId : entity.Id.ToString();

            if (entity is IBlogId blogId)
            {
                var blog = _memoryCache.Read<OrigamiBlog>().Id(blogId.BlogId);
                if (blog == null) throw new Exception($"Blog is null");
                return $"/files/blogs/{blog.NanoId}/{plural}/{directory}/";
            }

            if (entity is IBlogIdNull blogIdNull && blogIdNull.BlogId.HasValue)
            {
                var blog = _memoryCache.Read<OrigamiBlog>().Id(blogIdNull.BlogId.Value);
                if (blog == null) throw new Exception($"Blog is null");
                return $"/files/blogs/{blog.NanoId}/{plural}/{directory}/";
            }

            return $"/files/{plural}/{directory}/";
        }
    }
}
