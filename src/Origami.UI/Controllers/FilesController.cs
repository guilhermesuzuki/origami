using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models.FileSystem;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Text;

namespace Origami.UI.FrontEnd.Controllers
{
    [ApiController]
    public class FilesController :
        ControllerBase
    {
        protected readonly IBlogRepository _blogRepository;
        protected readonly IDirectoryRepository _directoryRepository;
        protected readonly IFileRepository _fileRepository;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="webHostEnvironment"></param>
        /// <param name="fileRepository"></param>
        public FilesController(
            IWebHostEnvironment webHostEnvironment,
            IDirectoryRepository directoryRepository,
            IFileRepository fileRepository,
            IBlogRepository blogRepository)
            : base()
        {
            _webHostEnvironment = webHostEnvironment;
            _directoryRepository = directoryRepository;
            _fileRepository = fileRepository;
            _blogRepository = blogRepository;
        }

        /// <summary>
        /// file sizes
        /// </summary>
        protected enum ePictureSizes : short
        {
            /// <summary>
            /// original (does not scale)
            /// </summary>
            original = 0,
            /// <summary>
            /// thumbnail (width: 50px)
            /// </summary>
            thumbnail = 50,
            /// <summary>
            /// small (width: 200px)
            /// </summary>
            small = 200,
            /// <summary>
            /// medium (width: 600px)
            /// </summary>
            medium = 600,
            /// <summary>
            /// large (width: 900px)
            /// </summary>
            large = 900,
        }

        [HttpGet]
        [Route("~/files/{*path}")]
        public async Task<IActionResult> IndexAsync(string path, [FromQuery] string? size)
        {
            try
            {
                //adds the files web directory to the virtual path
                var virtualpath = $"/files/{path.TrimStart('/')}";
                var file = _fileRepository.GetFile(virtualpath);
                if (file != null)
                {
                    if (file.IsImage)
                    {
                        var esize = ePictureSizes.original; Enum.TryParse(size, true, out esize);
                        return await PictureAsync(file, esize);
                    }

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
    }
}
