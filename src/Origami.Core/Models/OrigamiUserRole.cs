using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_UserRoles")]
    public class OrigamiUserRole :
        IChanged,
        IId
    {
        protected Guid _id = Guid.NewGuid();
        protected Guid _userId;
        protected Guid _roleId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiUserRole() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        /// <summary>
        /// User Id (FK)
        /// </summary>
        public Guid UserId
        {
            get { return _userId; }
            set { this.Set(ref _userId, value, Changed); }
        }

        /// <summary>
        /// Role Id (FK)
        /// </summary>
        public Guid RoleId
        {
            get { return _roleId; }
            set { this.Set(ref _roleId, value, Changed); }
        }
    }
}
