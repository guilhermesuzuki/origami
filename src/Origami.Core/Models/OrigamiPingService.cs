using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PingServices")]
    public class OrigamiPingService :
        IChanged,
        IId,
        IBlogIdNull
    {
        private Guid _id = Guid.NewGuid();
        private Guid? _blogId;
        private string _link = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPingService() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        public Guid? BlogId
        {
            get { return _blogId; }
            set { this.Set(ref _blogId, value, Changed); }
        }

        [StringLength(255)]
        public string Link
        {
            get { return _link; }
            set { this.Set(ref _link, value, Changed); }
        }
    }
}
