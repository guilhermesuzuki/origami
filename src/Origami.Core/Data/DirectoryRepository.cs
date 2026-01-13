using Origami.Core.Models;
using Origami.Core.Models.FileSystem;

namespace Origami.Core.Data
{
    public class DirectoryRepository :
        IDirectoryRepository
    {
        protected readonly IBlogRepository _blogRepository;
        protected readonly IWebRootPath _wwwRoot;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="wwwRoot"></param>
        public DirectoryRepository(
            IBlogRepository blogRepository,
            IWebRootPath wwwRoot)
            : base()
        {
            _blogRepository = blogRepository;
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

        public string WebPathForFiles<T>(T entity) where T : IId
        {
            if (entity is OrigamiSettings)
            {
                return $"/files/blogs/settings/";
            }

            var plural = typeof(T).GetPlural().ToLower();
            var directory = entity is INanoId nanoId ? nanoId.NanoId : entity.Id.ToString();

            if (entity is IBlogId blogId)
            {
                var blog = _blogRepository.ReadFromCache().Id(blogId.BlogId);
                if (blog == null) throw new Exception($"Blog is null");

                return $"/files/blogs/{blog.NanoId}/{plural}/{directory}/";
            }

            return $"/files/{plural}/{directory}/";
        }
    }
}
