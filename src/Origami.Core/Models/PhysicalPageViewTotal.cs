using System.ComponentModel;

namespace Origami.Core.Models
{
    public class PhysicalPageViewTotal :
        IChanged
    {
        private Guid _physicalPageId;
        private long _totalViews;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PhysicalPageViewTotal() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Gets or sets the unique identifier for the physical page.
        /// </summary>
        public Guid PhysicalPageId
        {
            get => _physicalPageId;
            set => this.Set(ref _physicalPageId, value, Changed);
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
