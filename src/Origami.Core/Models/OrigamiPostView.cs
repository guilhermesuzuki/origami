using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PostViews")]
    public class OrigamiPostView :
        BaseView,
        IViewChanged,
        IId,
        IPostId
    {
        protected Guid _id;
        protected Guid _postId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPostView() : base()
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

        public Guid PostId
        {
            get => _postId;
            set => this.Set(ref _postId, value, ViewChanged);
        }
    }
}
