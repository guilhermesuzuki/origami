using Microsoft.Extensions.Logging;
using Origami.Core.Data;
using Origami.Core.Models.FileSystem;
using Quartz;

namespace Origami.Core.Jobs
{
    [DisallowConcurrentExecution]
    public class EmptyFolderCleanUp(ISuperRepository Super, ILogger<EmptyFolderCleanUp> Logger) : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Executing EmptyFolderCleanUp job.");

            var directories = new List<string>();
            var blogs = Super.Blogs.ReadFromCache();

            foreach (var blog in blogs)
            {
                var pages = $"/files/blogs/{blog.NanoId}/pages/";
                var posts = $"/files/blogs/{blog.NanoId}/posts/";
                var softwareReleases = $"/files/blogs/{blog.NanoId}/software-releases/";
                var videos = $"/files/blogs/{blog.NanoId}/videos/";

                directories.Add(pages);
                directories.Add(posts);
                directories.Add(softwareReleases);
                directories.Add(videos);
            }

            directories.Add($"/files/special-pages/");

            foreach (var directory in directories)
            {
                if (Super.Directories.DirectoryExists(directory) == true)
                {
                    var dir = Super.Directories.GetDirectory(directory);
                    dir.Directories.Each(this._deleteEmptyFolders);
                }
            }

            return ValueTask.CompletedTask;
        }

        private void _deleteEmptyFolders(OrigamiSystemDirectory directory)
        {
            foreach (var subDirectory in directory.Directories)
            {
                this._deleteEmptyFolders(subDirectory);
            }

            if (directory.Directories.Count() == 0 && directory.Files.Count() == 0)
            {
                try
                {
                    Directory.Delete(directory.LocalPath);
                }
                catch
                {
                    // Ignore IO races (folder may have been populated/deleted concurrently)
                }
            }
        }
    }
}
