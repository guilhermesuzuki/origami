using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PostTags")]
    [Index(nameof(PostId), nameof(Tag), IsUnique = true, Name = "IX_oi_PostTags_1")]
    public class OrigamiPostTag :
        IChanged,
        IId,
        IFKPost,
        ITag,
        ISlug
    {
        private Guid _id = Guid.NewGuid();
        private OrigamiPost? _post;
        private Guid _postId;
        private string _tag = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPostTag() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        [ForeignKey(nameof(PostId))]
        public OrigamiPost? Post
        {
            get => _post;
            set => this.Set(ref _post, value, Changed);
        }

        public Guid PostId
        {
            get => _postId;
            set => this.Set(ref _postId, value, Changed);
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
