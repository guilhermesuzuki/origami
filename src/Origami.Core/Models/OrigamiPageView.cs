using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PageViews")]
    public class OrigamiPageView :
        BaseView,
        IViewChanged,
        IId,
        IPageId
    {
        protected Guid _id = Guid.NewGuid();
        protected Guid _pageId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPageView() : base()
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

        public Guid PageId
        {
            get => _pageId;
            set => this.Set(ref _pageId, value, ViewChanged);
        }
    }
}
