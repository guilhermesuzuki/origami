using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    public class OrigamiVideo :
        OrigamiContent,
        IVideo,
        IEmbedIFrame
    {
        protected OrigamiFile _mediaFile = new();
        protected OrigamiFile? _subtitle1;
        protected OrigamiFile? _subtitle2;
        protected OrigamiFile? _subtitle3;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideo() : base()
        {
            this.Type = nameof(OrigamiVideo);
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
            this.IsCommentEnabled = true;
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiVideo(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiVideoChanged = (sender, e) => { };

        [NotMapped]
        public string EmbedIFrame
        {
            get => Get().EmbedIFrame;
            set => Set(x => x.EmbedIFrame = value);
        }

        /// <summary>
        /// Media File for this Video
        /// </summary>
        public OrigamiFile MediaFile
        {
            get => _mediaFile;
            set => this.Set(ref _mediaFile, value, OrigamiVideoChanged);
        }

        /// <summary>
        /// Subtitle 1
        /// </summary>
        public OrigamiFile? Subtitle1
        {
            get => _subtitle1;
            set => this.Set(ref _subtitle1, value, OrigamiVideoChanged);
        }

        /// <summary>
        /// Subtitle 2
        /// </summary>
        public OrigamiFile? Subtitle2
        {
            get => _subtitle2;
            set => this.Set(ref _subtitle2, value, OrigamiVideoChanged);
        }

        /// <summary>
        /// Subtitle 3
        /// </summary>
        public OrigamiFile? Subtitle3
        {
            get => _subtitle3;
            set => this.Set(ref _subtitle3, value, OrigamiVideoChanged);
        }

        /// <summary>
        /// Extracts all subtitles in this video
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrigamiFile> GetSubtitles()
        {
            var result = new List<OrigamiFile>();

            if (Subtitle1 != null) result.Add(Subtitle1);
            if (Subtitle2 != null) result.Add(Subtitle2);
            if (Subtitle3 != null) result.Add(Subtitle3);

            return result;
        }
    }
}