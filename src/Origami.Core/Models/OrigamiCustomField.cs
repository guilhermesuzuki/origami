using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_CustomFields")]
    public class OrigamiCustomField :
        IChanged,
        IId,
        IBlogId
    {
        protected string _attribute = string.Empty;
        protected Guid _blogId;
        protected string _customType = string.Empty;
        protected Guid _id;
        protected string _key = string.Empty;
        protected string _objectId = string.Empty;
        protected string _value = string.Empty;

        public OrigamiCustomField() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// custom meta data like "hidden", "style" etc.
        /// </summary>
        [StringLength(250)]
        public string Attribute
        {
            get => _attribute;
            set => this.Set(ref _attribute, value, Changed);
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        /// <summary>
        /// Custom type, for example "post" or "theme"
        /// </summary>
        [StringLength(100)]
        public string CustomType
        {
            get => _customType;
            set => this.Set(ref _customType, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }
        /// <summary>
        /// The key in the key/value pair
        /// </summary>
        [StringLength(250)]
        public string Key
        {
            get => _key;
            set => this.Set(ref _key, value, Changed);
        }

        /// <summary>
        /// Object ID, for example post ID or theme name
        /// </summary>
        [StringLength(250)]
        public string ObjectId
        {
            get => _objectId;
            set => this.Set(ref _objectId, value, Changed);
        }
        /// <summary>
        /// The value. Can be simple string or string
        /// representation of object that client can parse
        /// </summary>
        public string Value
        {
            get => _value;
            set => this.Set(ref _value, value, Changed);
        }
    }
}
