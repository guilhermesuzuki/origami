using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_RightRoles")]
    public class OrigamiRightRole :
        IChanged,
        IId
    {
        protected Guid _id = Guid.NewGuid();
        protected Guid _rightId;
        protected Guid _roleId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiRightRole() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, Changed); }
        }

        /// <summary>
        /// Right Id (FK)
        /// </summary>
        public Guid RightId
        {
            get { return _rightId; }
            set { this.Set(ref _rightId, value, Changed); }
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
