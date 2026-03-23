using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_ContentReactions")]
    public class OrigamiContentReaction :
        BaseView,
        IReactionChanged,
        IId,
        IContentId
    {
        private Guid _id;
        private Guid _contentId;
        private string _reaction = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiContentReaction() : base()
        {
            Id = Guid.NewGuid();
        }

        public event EventHandler<PropertyChangedEventArgs> ReactionChanged = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ReactionChanged);
        }

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, ReactionChanged);
        }

        /// <summary>
        /// Reaction to this Comment
        /// </summary>
        [StringLength(5)]
        public string Reaction
        {
            get => _reaction;
            set => this.Set(ref _reaction, value, ReactionChanged);
        }
    }
}
