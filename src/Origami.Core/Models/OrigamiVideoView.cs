using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoViews")]
    public class OrigamiVideoView :
        BaseView,
        IViewChanged,
        IId,
        IVideoId
    {
        protected Guid _id;
        protected Guid _videoId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoView() : base()
        {
            Changed += (sender, e) => ViewChanged?.Invoke(this, e);
        }

        public event EventHandler<PropertyChangedEventArgs> ViewChanged = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ViewChanged);
        }

        public Guid VideoId
        {
            get => _videoId;
            set => this.Set(ref _videoId, value, ViewChanged);
        }
    }
}
