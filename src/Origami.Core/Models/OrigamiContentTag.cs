using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentTags")]
    public class OrigamiContentTag :
        IChanged,
        IId,
        IContentId,
        ITag,
        ISlug,
        IVersion,
        INew
    {
        private Guid _contentId;
        private Guid _id = Guid.NewGuid();
        private string _tag = string.Empty;
        protected byte[] _version = Array.Empty<byte>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentTag() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public bool New => _version.SequenceEqual(Array.Empty<byte>());

        public string Slug => Tag.GetSlug();

        /// <summary>
        /// Tag
        /// </summary>
        [StringLength(50)]
        public string Tag
        {
            get => _tag;
            set => this.Set(ref _tag, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
