using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.FileSystem;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;
using UAParser;

namespace Origami.UI.Controllers
{
    [ApiController]
    public class FilesController :
        ControllerBase
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IBlogRepository _blogRepository;
        protected readonly IDirectoryRepository _directoryRepository;
        protected readonly IFileRepository _fileRepository;
        protected readonly IMyMemoryCache _myMemoryCache;
        protected readonly IPhysicalPageRepository _physicalPageRepository;
        protected readonly IUserFacade _userFacade;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="webHostEnvironment"></param>
        /// <param name="fileRepository"></param>
        public FilesController(
            IWebHostEnvironment webHostEnvironment,
            IAppFacade appFacade,
            IDirectoryRepository directoryRepository,
            IFileRepository fileRepository,
            IBlogRepository blogRepository,
            IUserFacade userFacade,
            IPhysicalPageRepository physicalPageRepository,
            IMyMemoryCache myMemoryCache)
            : base()
        {
            _webHostEnvironment = webHostEnvironment;
            _blogRepository = blogRepository;
            _directoryRepository = directoryRepository;
            _fileRepository = fileRepository;
            _myMemoryCache = myMemoryCache;
            _physicalPageRepository = physicalPageRepository;
            _userFacade = userFacade;
            _appFacade = appFacade;
        }

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
                            var esize = ePictureSizes.original; Enum.TryParse(size, true, out esize);
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
                                    this._physicalPageRepository.View(virtualpath, view, this._userFacade.User);
                                }
                                else
                                {
                                    view.Admin = false;
                                    this._physicalPageRepository.View(virtualpath, view, this._userFacade.SocialProfile);
                                }
                                this._appFacade.RefreshUI(this.HttpContext.Connection.Id.ToString());
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
            var dontScale = file.WebPath.StartsWith(_blogRepository.DirectoryForScalingImages(), StringComparison.OrdinalIgnoreCase);
            if (dontScale || eSize == ePictureSizes.original)
            {
                return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
            }

            //image scaling
            if (eSize != ePictureSizes.original)
            {
                //first: get a md5 from file fullpath
                var utf8 = Encoding.UTF8.GetBytes(file.LocalPath);
                var hash = MD5.Create().ComputeHash(utf8).GetHexString();
                var directoryForScalingImages = _blogRepository.DirectoryForScalingImages();

                //creates the scaling directory
                _directoryRepository.Create(directoryForScalingImages);

                /*scaled file*/
                var scaleImageFilename = $"{hash}.{file.FileSize}.{eSize}{file.Extension}";
                var finalPath = $"{directoryForScalingImages}{scaleImageFilename}";
                var scaleImage = _fileRepository.GetFile(finalPath);

                //scale image does not exist or is out dated
                if (scaleImage == null || scaleImage.DateCreated != file.DateCreated || scaleImage.DateModified != file.DateModified)
                {
                    var fileScaled = await ScalePictureAsync(file, scaleImageFilename, eSize);
                    scaleImage = fileScaled ? _fileRepository.GetFile(finalPath) : null;
                }

                //there's a scaled image
                //verifies that the scaled image file size is smaller than the original file
                if (scaleImage != null && scaleImage.FileSize < file.FileSize)
                {
                    file = scaleImage;
                }
            }

            return PhysicalFile(file.LocalPath, file.ContentType, file.Name, true);
        }

        /// <summary>
        /// It scales the image to return as thumbnails and so on (depending on the request)
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> ScalePictureAsync(OrigamiSystemFile file, string filename, ePictureSizes eSize)
        {
            if (file == null) return false;
            if (file.IsImage == false) return false;

            var directory = _directoryRepository.GetDirectory(_blogRepository.DirectoryForScalingImages());
            var finalLocation = Path.Combine(directory.LocalPath, filename);

            try
            {
                using (var memstream = new MemoryStream(file.FileContents) { })
                using (Image image = Image.Load(memstream))
                {
                    var w = (short)eSize;
                    var f = 1 - (float)(image.Size.Width - (short)eSize) / image.Size.Width;
                    var h = (int)(image.Size.Height * f);

                    image.Mutate(x => x.Resize(w, h));

                    IImageEncoder? encoder = file.Extension.ToLower() switch
                    {
                        ".jpg" or ".jpeg" => new JpegEncoder
                        {
                            ColorType = JpegEncodingColor.Rgb,
                            Interleaved = true,
                            Quality = 50,
                            SkipMetadata = true,
                        },
                        ".png" => new PngEncoder
                        {
                            BitDepth = PngBitDepth.Bit8,
                            CompressionLevel = PngCompressionLevel.DefaultCompression,
                            SkipMetadata = true,
                            TransparentColorMode = PngTransparentColorMode.Preserve,
                        },
                        ".webp" => new WebpEncoder
                        {
                            Quality = 60,
                            SkipMetadata = true,
                        },
                        _ => null,
                    };

                    if (encoder != null)
                    {
                        await image.SaveAsync(finalLocation, encoder);
                    }
                    else
                    {
                        await image.SaveAsync(finalLocation);
                    }
                }
                return true;
            }
            catch
            {
                return false;
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

            var key = $"Origami_UserLocation_{this.HttpContext.Connection.Id}";
            tracking.Location = this._myMemoryCache.Get<Location>(key);
        }
    }
}
