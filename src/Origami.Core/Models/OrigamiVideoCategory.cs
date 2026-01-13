using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoCategories")]
    [Index(nameof(VideoId), nameof(CategoryId), IsUnique = true, Name = "IX_oi_VideoCategories_1")]
    public class OrigamiVideoCategory :
        IChanged,
        IId,
        ICategoryId,
        IFKVideo
    {
        protected Guid _categoryId;
        protected Guid _id = Guid.NewGuid();
        protected OrigamiVideo? _video;
        protected Guid _videoId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoCategory() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid CategoryId
        {
            get => _categoryId;
            set => this.Set(ref _categoryId, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
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
