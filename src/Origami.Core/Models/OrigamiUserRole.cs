using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_UserRoles")]
    public class OrigamiUserRole :
        IChanged,
        IId,
        IEnabled
    {
        protected bool _enabled = true;
        protected Guid _id = Guid.NewGuid();
        protected Guid _roleId;
        protected Guid _userId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiUserRole() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [NotMapped]
        public bool Enabled
        {
            get { return _enabled; }
            set { this.Set(ref _enabled, value, Changed); }
        }

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        /// <summary>
        /// Role Id (FK)
        /// </summary>
        public Guid RoleId
        {
            get { return _roleId; }
            set { this.Set(ref _roleId, value, Changed); }
        }

        /// <summary>
        /// User Id (FK)
        /// </summary>
        public Guid UserId
        {
            get { return _userId; }
            set { this.Set(ref _userId, value, Changed); }
        }
    }
}
