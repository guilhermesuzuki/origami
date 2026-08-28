using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_UserPasswordResets")]
    [Index(nameof(Key), IsUnique = true, Name = "UX_oi_UserPasswordResets_1")]
    public class OrigamiUserPasswordReset :
        IId,
        IChanged,
        IDeleted,
        IDateCreated,
        IAuthorId
    {
        protected Guid _authorId = Guid.NewGuid();
        protected DateTime _dateCreated;
        protected Guid _id = Guid.NewGuid();
        protected bool _isDeleted;
        protected string _key = string.Empty;
        protected Guid _userId = Guid.NewGuid();
        public OrigamiUserPasswordReset() : base()
        {
            this._key = Nanoid.Generate(size: 16);
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid AuthorId
        {
            get => _authorId;
            set => this.Set(ref _authorId, value, Changed);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => this.Set(ref _isDeleted, value, Changed);
        }

        [StringLength(16)]
        public string Key
        {
            get => _key;
            set => this.Set(ref _key, value, Changed);
        }

        public Guid UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, Changed);
        }
    }
}
