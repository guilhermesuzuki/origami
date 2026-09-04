using Microsoft.Extensions.Hosting;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.UI.Services
{
    public class EmptyFolderCleanUpService : TimerService
    {
        public EmptyFolderCleanUpService(ISuperRepository superRepository) : base(superRepository)
        {
        
        }

        protected override void TimeToDoSomething(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var blogs = this._super.Blogs.ReadFromCache();

            foreach (var blog in blogs)
            {
                var pagesPath = $"/files/blogs/{blog.NanoId}/pages/";
                var postsPath = $"/files/blogs/{blog.NanoId}/posts/";
                var softwareReleasesPath = $"/files/blogs/{blog.NanoId}/software-releases/";
                var videosPath = $"/files/blogs/{blog.NanoId}/videos/";

                if (this._super.Directories.DirectoryExists(pagesPath) == true)
                {
                    var pages = this._super.Directories.GetDirectory(pagesPath);
                    pages.Directories.Each(this._deleteEmptyFolders);
                }

                if (this._super.Directories.DirectoryExists(postsPath) == true)
                {
                    var posts = this._super.Directories.GetDirectory(postsPath);
                    posts.Directories.Each(this._deleteEmptyFolders);
                }

                if (this._super.Directories.DirectoryExists(softwareReleasesPath) == true)
                {
                    var softwareReleases = this._super.Directories.GetDirectory(softwareReleasesPath);
                    softwareReleases.Directories.Each(this._deleteEmptyFolders);
                }

                if (this._super.Directories.DirectoryExists(videosPath) == true)
                {
                    var videos = this._super.Directories.GetDirectory(videosPath);
                    videos.Directories.Each(this._deleteEmptyFolders);
                }
            }

            var specialPagesPath = $"/files/special-pages/";
            if (this._super.Directories.DirectoryExists(specialPagesPath) == true)
            {
                var specialPages = this._super.Directories.GetDirectory(specialPagesPath);
                specialPages.Directories.Each(this._deleteEmptyFolders);
            }
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
