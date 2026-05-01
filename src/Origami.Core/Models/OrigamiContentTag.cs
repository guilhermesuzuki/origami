using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentTags")]
    public class OrigamiContentTag :
        BaseModel,
        IChanged,
        IContentId,
        ITag,
        ISlug,
        IVersion,
        INew
    {
        protected Guid _contentId;
        protected string _tag = string.Empty;
        protected byte[] _version = Array.Empty<byte>();
        protected string _slug = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentTag() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) =>
        {
            
        };

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
        }

        public bool New => _version.SequenceEqual(Array.Empty<byte>());

        [StringLength(128)]
        public string Slug
        {
            get => _slug;
            set => this.Set(ref _slug, value, Changed);
        }

        /// <summary>
        /// Tag
        /// </summary>
        [StringLength(128)]
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
