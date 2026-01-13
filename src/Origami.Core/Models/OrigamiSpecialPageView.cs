using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_SpecialPageViews")]
    public class OrigamiSpecialPageView :
        BaseView,
        IViewChanged,
        IId
    {
        protected Guid _id = Guid.NewGuid();
        protected Guid _specialPageId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSpecialPageView() : base()
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

        public Guid SpecialPageId
        {
            get => _specialPageId;
            set => this.Set(ref _specialPageId, value, ViewChanged);
        }
    }
}
