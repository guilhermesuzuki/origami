using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoTags")]
    [Index(nameof(VideoId), nameof(Tag), IsUnique = true, Name = "IX_oi_VideoTags_1")]
    public class OrigamiVideoTag :
        IChanged,
        IId,
        IFKVideo,
        ITag,
        ISlug
    {
        private Guid _id;
        private string _tag = string.Empty;
        private OrigamiVideo? _video;
        private Guid _videoId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoTag() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public string Slug => Tag.GetSlug();

        /// <summary>
        /// Tag
        /// </summary>
        [StringLength(50)]
        public string Tag
        {
            get => _tag;
            set => this.Set(ref _tag, value, Changed);
        }

        [ForeignKey(nameof(VideoId))]
        public OrigamiVideo? Video
        {
            get => _video;
            set => this.Set(ref _video, value, Changed);
        }

        public Guid VideoId
        {
            get => _videoId;
            set => this.Set(ref _videoId, value, Changed);
        }
    }
}
