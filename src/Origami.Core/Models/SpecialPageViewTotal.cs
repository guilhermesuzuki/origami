using System.ComponentModel;

namespace Origami.Core.Models
{
    public class SpecialPageViewTotal :
        IChanged
    {
        protected Guid _specialPageId;
        protected long _totalViews;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SpecialPageViewTotal() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid SpecialPageId
        {
            get => _specialPageId;
            set => this.Set(ref _specialPageId, value, Changed);
        }

        /// <summary>
        /// Total number of views
        /// </summary>
        public long TotalViews
        {
            get => _totalViews;
            set => this.Set(ref _totalViews, value, Changed);
        }
    }
}
