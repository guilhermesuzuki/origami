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
        ISlug
    {
        private Guid _id = Guid.NewGuid();
        private Guid _contentId;
        private string _tag = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentTag() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
        }

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
    }
}
