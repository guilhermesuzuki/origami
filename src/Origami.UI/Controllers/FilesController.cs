using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NetVips;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.FileSystem;
using System.Security.Cryptography;
using System.Text;
using UAParser;

namespace Origami.UI.Controllers
{
    [ApiController]
    public class FilesController(
        IAppFacade _appFacade,
        IBlogRepository _blogRepository,
        IDirectoryRepository _directoryRepository,
        IFileRepository _fileRepository,
        IMyMemoryCache _myMemoryCache,
        IPhysicalPageRepository _physicalPageRepository,
        IUserFacade _userFacade,
        IWebHostEnvironment _webHostEnvironment) :
        ControllerBase
    {
        [HttpGet]
        [Route("~/files/{*path}")]
        public async Task<IActionResult> FilesAsync([FromRoute] string path, [FromQuery] string? size)
        {
            try
            {
                var virtualpath = $"/files/{path.TrimStart('/')}";
                var file = _fileRepository.GetFile(virtualpath);
                if (file != null)
                {
                    try
                    {
                        if (file.IsImage)
                        {
                            var esize = ePictureSizes.original;
                            _ = Enum.TryParse(size, true, out esize);
                            return await PictureAsync(file, esize);
                        }
                        return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
                    }
                    finally
                    {
                        try
                        {
                            if (virtualpath.PathComesFromSoftwareReleaseFiles() == true)
                            {
                                var view = new OrigamiPhysicalPageView();
                                this._fill(view);
                                if (_appFacade.Admin.GetValueOrDefault() == true)
                                {
                                    view.Admin = true;
                                    _physicalPageRepository.View(virtualpath, view, _userFacade.User);
                                }
                                else
                                {
                                    view.Admin = false;
                                    _physicalPageRepository.View(virtualpath, view, _userFacade.SocialProfile);
                                }
                                _userFacade.RefreshTheUI(OrigamiConstants.Events.UpdateCounters);
                            }
                        }
                        catch
                        {
                            // Best-effort tracking: ignore failures (e.g., DB unavailable) so file downloads still work.
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("~/files-backup/{*path}")]
        public async Task<IActionResult> FilesBackupAsync([FromRoute] string path, [FromQuery] string? size)
        {
            try
            {
                //adds the files web directory to the virtual path
                var virtualpath = $"/files-backup/{path.TrimStart('/')}";
                var file = _fileRepository.GetFile(virtualpath);
                if (file != null)
                {
                    return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
                }
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Returns the image/picture
        /// </summary>
        /// <param name="realPath"></param>
        /// <param name="eSize"></param>
        /// <returns></returns>
        protected async Task<FileResult> PictureAsync(OrigamiSystemFile file, ePictureSizes eSize)
        {
            if (eSize == ePictureSizes.original)
            {
                return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
            }

            var dontScale = file.WebPath.StartsWith(OrigamiSystemDirectory.DirectoryForScalingImages(), StringComparison.OrdinalIgnoreCase);
            if (dontScale)
            {
                return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
            }

            //image scaling
            var fileScaled = await ScalePictureAsync(file, eSize);

            if (fileScaled.Ok == true 
                && fileScaled.File != null 
                && fileScaled.File.FileSize < file.FileSize)
            {
                return PhysicalFile(fileScaled.File.LocalPath, fileScaled.File.ContentType, fileScaled.File.Name, true);
            }

            return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
        }

        /// <summary>
        /// It scales the image to return as thumbnails and so on (depending on the request)
        /// </summary>
        /// <returns></returns>
        protected async Task<(bool Ok, OrigamiSystemFile? File)> ScalePictureAsync(OrigamiSystemFile file, ePictureSizes eSize)
        {
            if (file == null) return (false, null);
            if (file.IsImage == false) return (false, null);

            var filename = file.ScaleFilename(eSize);
            var directory = _directoryRepository.GetDirectory(OrigamiSystemDirectory.DirectoryForScalingImages());
            var finalLocation = Path.Combine(directory.LocalPath, filename);
            var virtualPath = $"{OrigamiSystemDirectory.DirectoryForScalingImages()}{filename}";

            try
            {
                var existing = _fileRepository.GetFile(virtualPath);
                if (existing != null) return (true, existing);
                using var image = NetVips.Image.NewFromFile(file.LocalPath);
                using var resized = image.ThumbnailImage((int)eSize, 0, Enums.Size.Both, crop: NetVips.Enums.Interesting.None);
                resized.WriteToFile(finalLocation, new VOption { { "Q", 60 }, { "strip", true } });
                return (true, _fileRepository.GetFile(virtualPath));
            }
            catch
            {
                return (false, null);
            }
        }

        /// <summary>
        /// Fills the <paramref name="tracking"/> with request information
        /// </summary>
        /// <param name="tracking"></param>
        /// <param name="url"></param>
        /// <param name="referrer"></param>
        private void _fill(BaseTracking tracking)
        {
            var dd = Request.GetDeviceDetector();

            // important!
            dd.Parse();

            tracking.DateCreated = DateTime.UtcNow;
            tracking.UserAgent = HttpContext.Request.Header("User-Agent");
            tracking.HostAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            tracking.IsMobileDevice = dd.IsTablet() || dd.IsMobile();
            tracking.IsBot = dd.IsBot();
            tracking.SocialProfileId = _userFacade.SocialProfile.New == false ? _userFacade.SocialProfile.Id : null;

            var client = Parser.GetDefault().Parse(tracking.UserAgent);

            tracking.Platform = client.OS.Family;
            tracking.Browser = client.UA.Family;

            var key = $"Origami_UserLocation_{HttpContext.Connection.Id}";
            tracking.Location = _myMemoryCache.Get<Location>(key);
        }
    }
}
