using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public abstract class BaseModel : IId, INanoId
    {
        protected Guid _id = Guid.Empty;
        protected string _nanoId = string.Empty;

        public event EventHandler<PropertyChangedEventArgs> BaseModelChanged = (sender, p) => { };

        protected BaseModel()
        {
            this._id = Guid.CreateVersion7(DateTimeOffset.UtcNow);
            this._nanoId = Nanoid.Generate(Nanoid.Alphabets.LettersAndDigits, 8);
        }

        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public virtual Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, BaseModelChanged);
        }

        /// <summary>
        /// Nano Id
        /// </summary>
        [StringLength(8)]
        public virtual string NanoId
        {
            get => _nanoId;
            set => this.Set(ref _nanoId, value, BaseModelChanged);
        }
    }
}
