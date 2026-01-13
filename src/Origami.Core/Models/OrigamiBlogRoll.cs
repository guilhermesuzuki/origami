using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_BlogRolls")]
    public class OrigamiBlogRoll :
        IChanged,
        IId,
        ITitle,
        IDescriptionNull,
        ISortOrder
    {
        protected string _blogUrl = string.Empty;
        protected string? _description = string.Empty;
        protected string _feedUrl = string.Empty;
        protected Guid _id;
        protected int _sortOrder;
        protected string _title = string.Empty;
        protected string _xfn = string.Empty;

        public OrigamiBlogRoll() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [StringLength(255)]
        public string BlogUrl
        {
            get => _blogUrl;
            set => this.Set(ref _blogUrl, value, Changed);
        }

        [StringLength(255)]
        public string? Description
        {
            get => _description;
            set => this.Set(ref _description, value, Changed);
        }

        [StringLength(255)]
        public string FeedUrl
        {
            get => _feedUrl;
            set => this.Set(ref _feedUrl, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }
        public int SortOrder
        {
            get => _sortOrder;
            set => this.Set(ref _sortOrder, value, Changed);
        }

        [StringLength(255)]
        public string Title
        {
            get => _title;
            set => this.Set(ref _title, value, Changed);
        }
        [StringLength(255)]
        public string Xfn
        {
            get => _xfn;
            set => this.Set(ref _xfn, value, Changed);
        }
    }
}
