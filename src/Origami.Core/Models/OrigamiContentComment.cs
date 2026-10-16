using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentComments")]
    public class OrigamiContentComment :
        BaseComment,
        ICommentChanged,
        IContentId
    {
        protected Guid _contentId;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentComment() : base()
        {
            this.Id = Guid.NewGuid();
            this.IsApproved = true;
        }

        public event EventHandler<PropertyChangedEventArgs> CommentChanged = (sender, e) => { };

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, CommentChanged);
        }
    }
}
