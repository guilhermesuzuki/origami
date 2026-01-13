using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PhysicalPageReactions")]
    public class OrigamiPhysicalPageReaction :
        BaseView,
        IReactionChanged,
        IId,
        IPhysicalPageId
    {
        protected Guid _id = Guid.NewGuid();
        protected Guid _physicalPageId;
        protected string _reaction = string.Empty;

        public event EventHandler<PropertyChangedEventArgs> ReactionChanged = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, ReactionChanged);
        }

        public Guid PhysicalPageId
        {
            get => _physicalPageId;
            set => this.Set(ref _physicalPageId, value, ReactionChanged);
        }

        /// <summary>
        /// Reaction to this Physical Page
        /// </summary>
        [StringLength(5)]
        public string Reaction
        {
            get => _reaction;
            set => this.Set(ref _reaction, value, ReactionChanged);
        }
    }
}
