using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NanoidDotNet;
using Origami.Core;
using Origami.Core.Models;
using Origami.Core.Models.FileSystem;
using Polly;
using Polly.Retry;
using SixLabors.ImageSharp;
using System.Buffers;

namespace Origami.UI
{
    public class BasicForm<T> :
        Basic,
        IEntity<T>,
        ICreateEntityVoid<T>,
        ISave
        where T : class, IId, new()
    {
        /// <summary>
        /// Should show or hide the file manager for picking images
        /// </summary>
        protected bool FileManagerForImages;

        /// <summary>
        /// Should show or hide the file manager for picking videos
        /// </summary>
        protected bool FileManagerForVideos;

        /// <summary>
        /// Indicates whether a file upload operation is currently in progress.
        /// </summary>
        /// <remarks>This field is intended for use within derived classes to track the state of file
        /// upload operations.</remarks>
        protected bool FileUploading = false;

        /// <summary>
        /// The name of the file currently being uploaded. This field is intended for use within derived classes to track the name of the file being uploaded.
        /// </summary>
        protected string FileUploadingName = string.Empty;

        /// <summary>
        /// Video uploading progress
        /// </summary>
        protected int FileUploadingProgress;

        /// <summary>
        /// This <see cref="CancellationTokenSource"/> is used to control the process of uploading a video
        /// </summary>
        protected CancellationTokenSource FileUploadingToken = new();

        [Parameter] public EventCallback<T> Cancelled { get; set; }
        [Parameter] public EventCallback<T> Created { get; set; }
        [Parameter] public T Entity { get; set; } = Activator.CreateInstance<T>();
        [Parameter] public EventCallback<T> Saved { get; set; }

        /// <summary>
        /// Author from the Entity (when available)
        /// </summary>
        protected virtual OrigamiUser Author
        {
            get
            {
                if (Entity is IAuthorId author)
                {
                    return Super.Users.ReadFromCache().Id(author.AuthorId) ?? new() { Username = "Unknown" };
                }
                return new() { Username = "Unknown" };
            }
        }

        /// <summary>
        /// Rules for disabling the New button
        /// </summary>
        protected virtual bool DisableTheNewButton => false;

        /// <summary>
        /// Rules for disabling the Save button
        /// </summary>
        protected virtual bool DisableTheSaveButton => false;

        /// <summary>
        /// Shows or hides the parent selector
        /// </summary>
        protected bool ShowParentSelector { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="Entity"/>
        /// </summary>
        public async Task CreateEntity()
        {
            try
            {
                Entity = TheCreator.Create<T>();
                await Created.InvokeAsync(Entity);
            }
            finally
            {
                ShowParentSelector = false;
            }
        }

        /// <summary>
        /// Saves the entity in the database, updating cache
        /// </summary>
        public virtual void Save() { }

        public virtual void SetParent(IId entity)
        {
            if (Entity is IParentIdNull parent)
            {
                parent.ParentId = entity.Id;
                return;
            }

            throw new NotImplementedException("Entity does not support parent");
        }
        /// <summary>
        /// Cancels the edit
        /// </summary>
        public virtual void UndoChanges() { }

        /// <summary>
        /// Clears the header image, setting it to a default value
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void ClearHeader()
        {
            if (Entity is IHeaderImage header)
            {
                header.HeaderImage = Entity switch
                {
                    OrigamiUser => OrigamiConstants.NoUser,
                    OrigamiBlog => OrigamiConstants.NoHeader,
                    OrigamiPage => OrigamiConstants.NoHeader,
                    OrigamiPost => OrigamiConstants.NoHeader,
                    OrigamiVideo => OrigamiConstants.NoHeader,
                    OrigamiCategory => OrigamiConstants.NoCategory,
                    _ => string.Empty,
                };
                return;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the video
        /// </summary>
        protected virtual void ClearVideo()
        {
            if (Entity is OrigamiVideo video)
            {
                video.MediaFile = new();
                return;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// An image was picked from the file manager and should be assigned in the entity as a header
        /// </summary>
        /// <param name="image"></param>
        protected virtual void ImageFromFileManagerWillAssignEntityHeader(OrigamiSystemFile image)
        {
            if (image.IsImage == false)
            {
                UserFacade.Result = new() { Error = Text.Original("You need to pick an image") };
                return;
            }

            if (Entity is IHeaderImage header)
            {
                if (image.WebPath.Like(header.HeaderImage) == true)
                {
                    FileManagerForImages = false;
                    UserFacade.Result = new() { Info = Text.Original("Source and destination images share the same location") };
                    return;
                }

                var sourcePath = Super.Files.LocalPath(image.WebPath);
                var webPath = Super.Directories.WebPathForFiles(Entity);
                var localPath = Super.Directories.LocalPathForFiles(Entity);

                if (sourcePath.StartsWith(localPath) == true)
                {
                    header.HeaderImage = image.WebPath;
                }
                else
                {
                    try
                    {
                        var filename = Path.GetFileName(sourcePath);
                        var destinationPath = Super.Files.LocalPath($"{webPath}{filename}");
                        if (System.IO.File.Exists(destinationPath) == true)
                        {
                            var extension = Path.GetExtension(destinationPath);
                            var newfilename = $"{Path.GetFileNameWithoutExtension(destinationPath)}.{Nanoid.Generate(Nanoid.Alphabets.UppercaseLettersAndDigits, 4)}{extension}";
                            destinationPath = Super.Files.LocalPath($"{webPath}{newfilename}");
                        }
                        if (System.IO.File.Exists(destinationPath) == true)
                        {
                            throw new InvalidOperationException("File with the same name exists. Please, try again");
                        }
                        header.HeaderImage = $"{webPath}{Path.GetFileName(destinationPath)}";
                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (destinationDirectory.Has() == true)
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }
                        System.IO.File.Copy(sourcePath, destinationPath, true);
                    }
                    catch (Exception ex)
                    {
                        UserFacade.Result = new(ex);
                    }
                }

                FileManagerForImages = false;
            }
            else
            {
                FileManagerForImages = false;
                UserFacade.Result = new() { Error = Text.Original("Entity header cannot be updated") };
            }
        }

        /// <summary>
        /// An image was picked from the file manager and should be assigned in the entity as a header as base64 string
        /// </summary>
        /// <param name="image"></param>
        protected async Task ImageFromFileManagerWillAssignEntityHeaderAsBase64(OrigamiSystemFile image)
        {
            if (image.IsImage == false)
            {
                this.UserFacade.Result = new() { Error = Text.Original("You need to pick an image") };
                return;
            }

            if (this.Entity is IHeaderImage header)
            {
                var localPath = this.Super.Files.LocalPath(image.WebPath);

                using var reader = File.OpenRead(localPath);
                using var memoryStream = new MemoryStream();
                await reader.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var extension = Path.GetExtension(image.Name).TrimStart('.');
                header.HeaderImage = $"data:image/{extension};base64,{Convert.ToBase64String(imageBytes)}";
            }

            this.FileManagerForImages = false;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Entity.SetAuthor(UserFacade.User);
            Entity.SetBlog(GetBlogFromUserFacade());
        }

        /// <summary>
        /// Uploads a file to the server, saving it locally and generating a web-accessible path.
        /// </summary>
        /// <remarks>This method handles file uploads by saving the file locally and optionally processing
        /// it if it is an image. If the file already exists at the target location, a unique name will be generated to
        /// avoid overwriting. The method updates the upload progress and ensures proper cleanup in case of
        /// errors.</remarks>
        /// <param name="file">The file to be uploaded. Must implement <see cref="IBrowserFile"/>.</param>
        /// <param name="filename">The name to assign to the uploaded file. If the file already exists, a unique name will be generated.</param>
        /// <param name="fileLimit">The maximum allowed file size, in bytes. Files exceeding this limit will result in an exception.</param>
        /// <returns>A tuple containing the following: <list type="bullet"> <item><term><c>Ok</c></term><description><see
        /// langword="true"/> if the upload was successful; otherwise, <see langword="false"/>.</description></item>
        /// <item><term><c>LocalPath</c></term><description>The local file path where the file was saved. Empty if the
        /// upload failed.</description></item> <item><term><c>WebPath</c></term><description>The web-accessible path
        /// for the uploaded file. Empty if the upload failed.</description></item> </list></returns>
        /// <exception cref="InvalidOperationException">Thrown if the file size exceeds <paramref name="fileLimit"/>.</exception>
        protected async Task<(bool Ok, string LocalPath, string WebPath)> UploadFile(IBrowserFile file, long fileLimit, string? filename = null, string? subDirectoryName = null)
        {
            if (file.Size > fileLimit)
            {
                throw new InvalidOperationException(Text.Get("File is too large"));
            }

            this.FileUploading = true;
            this.FileUploadingName = file.Name;

            // Set up timer for UI updates
            using var timer = new Timer(_ => InvokeAsync(StateHasChanged));
            timer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

            filename = filename ?? file.Name;

            string basePath = Super.Directories.LocalPathForFiles(Entity);

            if (subDirectoryName.Has() == true)
            {
                basePath = Path.Combine(basePath, subDirectoryName);
            }

            string tempPath = Path.Combine(basePath, $"{Guid.NewGuid()}.tmp");
            string finalPath = Path.Combine(basePath, filename);

            try
            {
                var pipeline = new ResiliencePipelineBuilder()
                    .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1024, Delay = TimeSpan.FromMilliseconds(10), })
                    .Build();

                await pipeline.ExecuteAsync(token =>
                {
                    if (File.Exists(finalPath) == true)
                    {
                        var newFilename = $"{Path.GetFileNameWithoutExtension(filename)}.{Nanoid.Generate(Nanoid.Alphabets.UppercaseLettersAndDigits, 4)}{Path.GetExtension(filename)}";
                        finalPath = Path.Combine(basePath, newFilename);
                        throw new Exception("File already exists");
                    }
                    return ValueTask.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
                return (false, string.Empty, string.Empty);
            }

            await using Stream stream = file.OpenReadStream(file.Size, FileUploadingToken.Token);

            const int bufferSize = 1024 * 1024; // 1 MB
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            long fileBytesRead = 0;

            try
            {
                Directory.CreateDirectory(basePath);
                using (FileStream fs = File.Create(tempPath))
                {
                    int bytesRead;
                    while ((bytesRead = await stream.ReadAsync(buffer, FileUploadingToken.Token)) != 0)
                    {
                        fileBytesRead += bytesRead;
                        FileUploadingProgress = (int)(100 * fileBytesRead / file.Size);
                        await fs.WriteAsync(buffer.AsMemory(0, bytesRead));
                    }
                }

                if (finalPath.IsImage() == true)
                {
                    using (Image image = Image.Load(tempPath))
                    {
                        image.Save(finalPath);
                    }
                }
                else
                {
                    File.Copy(tempPath, finalPath, true);
                }

                FileUploadingProgress = 100;

                var webPath = $"{Super.Directories.WebPathForFiles(Entity)}{Path.GetFileName(finalPath)}";

                return (true, finalPath, webPath);
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
                return (false, string.Empty, string.Empty);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                FileUploading = false;
                FileUploadingProgress = 0;
                File.Delete(tempPath);
            }
        }

        /// <summary>
        /// Handles the processing of a cropped logo image and updates the entity's logo URL.
        /// </summary>
        /// <remarks>This method decodes the provided Base64 image, saves it as a PNG file in the
        /// appropriate directory,  and updates the entity's logo URL to point to the newly saved file. If a file with
        /// the default name  already exists, a unique filename is generated to avoid overwriting existing
        /// files.</remarks>
        /// <param name="base64Logo">A Base64-encoded string representing the cropped image data.</param>
        /// <returns></returns>
        protected async Task UploadHeader(string base64Logo)
        {
            if (Entity is IHeaderImage header)
            {
                try
                {
                    var wpath = Super.Directories.WebPathForFiles(Entity);
                    var lpath = Super.Directories.LocalPath(wpath);
                    var filename = $"logo.png";

                    if (File.Exists(Path.Combine(lpath, filename)) == true)
                    {
                        filename = $"logo.{Nanoid.Generate(Nanoid.Alphabets.UppercaseLettersAndDigits, 4)}.png";
                    }

                    if (File.Exists(Path.Combine(lpath, filename)) == true)
                    {
                        throw new InvalidOperationException("File with the same name exists. Please, try again");
                    }

                    var bytes = base64Logo.Base64ImageToBytes();
                    var image = Image.Load(bytes);
                    Directory.CreateDirectory(lpath);
                    await image.SaveAsPngAsync(lpath + filename);
                    header.HeaderImage = wpath + filename;
                }
                catch (Exception ex)
                {
                    UserFacade.Result = new(ex);
                }
                return;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Upload header image (2MB max)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual async Task UploadHeader(IBrowserFile file)
        {
            try
            {
                if (Entity is IHeaderImage headerImage)
                {
                    var status = await this.UploadFile(file, OrigamiConstants.MaximumFileSizeForHeaderImages);
                    if (status.Ok)
                    {
                        headerImage.HeaderImage = status.WebPath;
                        UserFacade.Result = new();
                    }
                    return;
                }
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
            }
        }

        /// <summary>
        /// Upload header image in base64 format (512KB max)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual async Task UploadHeaderBase64(IBrowserFile file)
        {
            if (file.Size > OrigamiConstants.MaximumBase64StringForHeaderImages)
            {
                UserFacade.Result = new() { Error = Text.Original("File is too large") };
                return;
            }

            using var reader = file.OpenReadStream(file.Size);
            using var memoryStream = new MemoryStream();
            await reader.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            if (Entity is IHeaderImage headerImage)
            {
                var extension = Path.GetExtension(file.Name).TrimStart('.');
                headerImage.HeaderImage = $"data:image/{extension};base64,{Convert.ToBase64String(imageBytes)}";
            }
        }

        /// <summary>
        /// Upload video (2GB max by default)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual async Task UploadVideo(IBrowserFile file)
        {
            if (Entity is OrigamiVideo video)
            {
                try
                {
                    var status = await UploadFile(file, OrigamiConstants.MaximumFileSizeForVideos);
                    if (status.Ok)
                    {
                        video.MediaFile.LocalPath = status.LocalPath;
                        video.MediaFile.WebPath = status.WebPath;
                    }
                }
                catch (Exception ex)
                {
                    UserFacade.Result = new(ex);
                }
            }
        }

        /// <summary>
        /// A video was picked from the file manager and should be assigned in the entity as a header
        /// </summary>
        /// <param name="videofile"></param>
        protected virtual void VideoFromFileManagerWillAssignEntityHeader(OrigamiSystemFile video)
        {
            if (video.IsVideo == false)
            {
                UserFacade.Result = new() { Error = Text.Original("You need to pick a video") };
                return;
            }

            if (Entity is OrigamiVideo oiVideo)
            {
                if (video.WebPath.Like(oiVideo.MediaFile.WebPath) == true)
                {
                    FileManagerForImages = false;
                    UserFacade.Result = new() { Info = Text.Original("Source and destination images share the same location") };
                    return;
                }

                var sourcePath = Super.Files.LocalPath(video.WebPath);
                var webPath = Super.Directories.WebPathForFiles(Entity);
                var localPath = Super.Directories.LocalPathForFiles(Entity);

                if (sourcePath.StartsWith(localPath) == true)
                {
                    oiVideo.HeaderImage = video.WebPath;
                }
                else
                {
                    try
                    {
                        var filename = Path.GetFileName(sourcePath);
                        var destinationPath = Super.Files.LocalPath($"{webPath}{filename}");
                        if (File.Exists(destinationPath) == true)
                        {
                            var extension = Path.GetExtension(destinationPath);
                            var newfilename = $"{Path.GetFileNameWithoutExtension(destinationPath)}.{Nanoid.Generate(Nanoid.Alphabets.UppercaseLettersAndDigits, 4)}{extension}";
                            destinationPath = Super.Files.LocalPath($"{webPath}{newfilename}");
                        }
                        if (File.Exists(destinationPath) == true)
                        {
                            throw new InvalidOperationException("File with the same name exists. Please, try again");
                        }
                        oiVideo.HeaderImage = $"{webPath}{Path.GetFileName(destinationPath)}";
                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (destinationDirectory.Has() == true)
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }
                        File.Copy(sourcePath, destinationPath, true);
                    }
                    catch (Exception ex)
                    {
                        UserFacade.Result = new(ex);
                    }
                }

                FileManagerForImages = false;
            }
            else
            {
                FileManagerForImages = false;
                UserFacade.Result = new() { Error = Text.Original("Video cannot be updated") };
            }
        }
    }
}
