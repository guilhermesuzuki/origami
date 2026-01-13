using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Rights")]
    [Index(nameof(Name), IsUnique = true, Name = "UX_oi_Rights_1")]
    public class OrigamiRight :
        IChanged,
        IId,
        IName
    {
        protected Guid _id = Guid.NewGuid();
        protected string _name = string.Empty;

        public OrigamiRight() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        [StringLength(100)]
        public string Name
        {
            get { return _name; }
            set { this.Set(ref _name, value, Changed); }
        }
    }
}
