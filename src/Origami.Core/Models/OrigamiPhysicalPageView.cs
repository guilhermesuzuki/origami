using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PhysicalPageViews")]
    public class OrigamiPhysicalPageView :
        BaseView,
        IViewChanged,
        IId,
        IPhysicalPageId,
        IAdmin
    {
        protected bool? _admin;
        protected Guid _id = Guid.NewGuid();
        protected Guid _physicalPageId = Guid.Empty;
        protected Guid? _userId;
        private Content? _content;

        public event EventHandler<PropertyChangedEventArgs> ViewChanged = (sender, e) => { };

        /// <summary>
        /// Is this view from an admin page?
        /// </summary>
        public bool? Admin
        {
            get => _admin;
            set => this.Set(ref _admin, value, ViewChanged);
        }

        /// <summary>
        /// Sometimes a physical page view is tied to a page, post, video, etc.
        /// </summary>
        public Content? Content
        {
            get => _content;
            set => this.Set(ref _content, value, ViewChanged);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ViewChanged);
        }

        public Guid PhysicalPageId
        {
            get => _physicalPageId;
            set => this.Set(ref _physicalPageId, value, ViewChanged);
        }

        /// <summary>
        /// Gets or sets the unique identifier of the user associated with this instance.
        /// </summary>
        public Guid? UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, ViewChanged);
        }
    }
}
