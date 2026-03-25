using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// This class is used to represent Post and Video Tags
    /// </summary>
    public class OrigamiTag :
        IChanged,
        IId,
        INew,
        IBlogIdNull,
        ISlug,
        ITag
    {
        private Guid? _blogId;
        private Guid _id = Guid.NewGuid();
        private string _tag = string.Empty;

        public OrigamiTag() { }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid? BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// Tag name
        /// </summary>
        public string Tag
        {
            get => _tag;
            set => this.Set(ref _tag, value, Changed);
        }

        /// <summary>
        /// Always false, because we are not adding a Tag
        /// </summary>
        public bool New => _tag.Has() == false;

        [NotMapped]
        public string Slug => Tag.GetSlug();
    }
}
