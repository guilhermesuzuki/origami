using System.ComponentModel;

namespace Origami.Core.Models
{
    public class PageViewTotal :
        IChanged,
        IPageId
    {
        protected Guid _pageId;
        protected long _totalViews;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageViewTotal() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid PageId
        {
            get => _pageId;
            set => this.Set(ref _pageId, value, Changed);
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
