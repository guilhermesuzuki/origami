using System.ComponentModel;

namespace Origami.Core.Models.FileSystem
{
    public class OrigamiSystemFile :
        IId,
        IChanged,
        ILocalPath,
        IWebPath,
        IDateCreated,
        IDateModified,
        IChecked
    {
        /// <summary>
        /// List of valid audio extensions
        /// </summary>
        protected string[] _audioExtensions = {
            ".3gp",".aa",".aac",".aax",".act",".aiff",".alac",".amr",".ape",".au",
            ".awb",".dss",".dvf",".flac",".gsm",".iklax",".ivs",".m4a",".m4b",".m4p",
            ".mmf",".mp3",".mpc",".msv",".nmf",".oga",".mogg",".opus",".ra",
            ".rm",".raw",".rf64",".tta",".voc",".vox",".wav",".wma",".wv",
            ".8svx",".cda",
        };

        protected bool _checked;
        /// <summary>
        /// Date and time the file was created
        /// </summary>
        protected DateTime _dateCreated = DateTime.MinValue;

        /// <summary>
        /// Date and time the file was modified
        /// </summary>
        protected DateTime? _dateModified;

        /// <summary>
        /// the file size, in raw long format
        /// </summary>
        protected long _fileSize;

        protected Guid _id = Guid.NewGuid();

        /// <summary>
        /// list of valid image extensions
        /// </summary>
        protected string[] _imageExtensions = { ".bmp", ".jpg", ".jpeg", ".png", ".tiff" };

        /// <summary>
        /// the full path of the file, internal field only, use file path for external calls. reduces security concerns
        /// while outside of the buisness layer
        /// </summary>
        protected string _localPath = string.Empty;

        /// <summary>
        /// list of valid video extensions
        /// </summary>
        protected string[] _videoExtensions = {
            ".webm", ".mkv", ".flv", ".vob", ".ogv", ".ogg", ".drc", ".gif", ".gifv",
            ".mng", ".avi", ".mov", ".qt", ".wmv", ".yuv", ".rm", ".rmv", ".asf",
            ".asf", ".amv", ".mp4", ".m4p", ".m4v", ".mpg", ".mp2", ".mpeg", ".mpe", ".mpv",
            ".mpg", ".mpeg", ".m2v", ".m4v", ".svi", ".3gp", ".3g2", ".mxf", ".roq", ".nsv",
            ".flv", ".f4v", ".f4p", ".f4a", ".f4b",
            };

        /// <summary>
        /// web path of the file
        /// </summary>
        protected string _webPath = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localPath"></param>
        public OrigamiSystemFile(string localPath) : base()
        {
            var fileInfo = new FileInfo(localPath);
            if (fileInfo.Exists == false)
            {
                throw new FileNotFoundException($"File {localPath} does NOT exist");
            }

            LocalPath = localPath;
            FileSize = fileInfo.Length;
            DateCreated = fileInfo.CreationTimeUtc;
            DateModified = fileInfo.LastWriteTimeUtc;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localPath"></param>
        public OrigamiSystemFile(string localPath, string webPath) : this(localPath)
        {
            WebPath = webPath;
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public bool Checked
        {
            get => _checked;
            set => this.Set(ref _checked, value, Changed);
        }

        public string ContentType
        {
            get
            {
                var extension = Extension.TrimStart('.').ToLower();

                if (this.IsImage)
                {
                    return string.Compare(extension, "jpg", true) == 0 ? "image/jpeg" : $"image/{extension}";
                }
                else if (this.IsVideo)
                {
                    return extension switch
                    {
                        "3g2" => "video/3gpp2",
                        "3gp" => "video/3gpp",
                        "avi" => "video/x-msvideo",
                        "flv" => "video/x-flv",
                        "m4v" => "video/x-m4v",
                        "mkv" => "video/x-matroska",
                        "mov" => "video/quicktime",
                        "mp4" => "video/mp4",
                        "ogg" => "video/ogg",
                        "ogv" => "video/ogg",
                        "ts" => "video/mp2t",
                        "webm" => "video/webm",
                        "wmv" => "video/x-ms-wmv",
                        _ => "application/octet-stream"
                    };
                }
                else if (this.IsAudio)
                {
                    return extension switch
                    {
                        "aac" => "audio/aac",
                        "flac" => "audio/flac",
                        "m4a" => "audio/mp4",
                        "mp3" => "audio/mpeg",
                        "oga" => "audio/ogg",
                        "ogg" => "audio/ogg",
                        "opus" => "audio/opus",
                        "wav" => "audio/wav",
                        "weba" => "audio/webm",
                        _ => "application/octet-stream"
                    };
                }

                return "application/octet-stream";
            }
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        /// <summary>
        /// gets the file extension
        /// </summary>
        public string Extension
        {
            get => Path.GetExtension(Name);
        }

        /// <summary>
        /// Gets the File Contents
        /// </summary>
        public byte[] FileContents
        {
            get => _localPath.ToBytes();
        }

        /// <summary>
        /// gets the file size, in raw long
        /// </summary>
        public long FileSize
        {
            get => _fileSize;
            private set => this.Set(ref _fileSize, value, Changed);
        }

        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// validates if this file is an audio
        /// </summary>
        public bool IsAudio
        {
            get => _audioExtensions.Any(x => x.ToLower() == Extension.ToLower());
        }

        /// <summary>
        /// valdidates if this object is an image
        /// </summary>
        public bool IsImage
        {
            get => _imageExtensions.Any(x => x.ToLower() == Extension.ToLower());
        }

        /// <summary>
        /// valdidates if this object is a video
        /// </summary>
        public bool IsVideo
        {
            get => _videoExtensions.Any(x => x.ToLower() == Extension.ToLower());
        }

        /// <summary>
        /// gets the full path. To change the path use rename methods
        /// </summary>
        public string LocalPath
        {
            get => _localPath;
            private set => this.Set(ref _localPath, value, Changed);
        }

        /// <summary>
        /// gets the file name
        /// </summary>
        /// <remarks>
        /// set accessor set to the internal
        /// </remarks>
        public string Name
        {
            get => Path.GetFileName(_localPath);
        }

        /// <summary>
        /// Full web path of the file
        /// </summary>
        public string WebPath
        {
            get => _webPath;
            private set => this.Set(ref _webPath, value, Changed);
        }

        /// <summary>
        /// Only the directory of the web path
        /// </summary>
        public string WebPathDirectory
        {
            get
            {
                var webPath = WebPath.TrimEnd('/');
                return webPath.Substring(0, webPath.LastIndexOf('/'));
            }
        }
    }
}
