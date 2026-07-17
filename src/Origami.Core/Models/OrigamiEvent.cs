using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Events")]
    public abstract class OrigamiEvent :
        BaseModel,
        IDateCreated,
        IType,
        ISubtypeNull,
        IUserIdNull,
        ISocialProfileIdNull
    {
        protected DateTime _dateCreated;
        protected Guid? _socialProfileId;
        protected string? _subtype;
        protected string _type = string.Empty;
        protected Guid? _userId;

        public event EventHandler<PropertyChangedEventArgs> EventChanged = (sender, p) => { };

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, EventChanged);
        }

        public Guid? SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, EventChanged);
        }

        [StringLength(64)]
        public string? Subtype
        {
            get => _subtype;
            set => this.Set(ref _subtype, value, EventChanged);
        }

        [StringLength(64)]
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, EventChanged);
        }

        public Guid? UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, EventChanged);
        }
    }
}
