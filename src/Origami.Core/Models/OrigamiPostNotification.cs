using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PostNotifications")]
    [Index(nameof(PostId), nameof(Email), IsUnique = true, Name = "IX_oi_PostNotifications_1")]
    public class OrigamiPostNotification :
        IChanged,
        IId,
        IFKPost,
        IEmail
    {
        private Guid _id;
        private Guid _postId;
        private OrigamiPost? _post;

        private string _email = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPostNotification() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public Guid PostId
        {
            get => _postId;
            set => this.Set(ref _postId, value, Changed);
        }

        [ForeignKey(nameof(PostId))]
        public OrigamiPost? Post
        {
            get => _post;
            set => this.Set(ref _post, value, Changed);
        }

        /// <summary>
        /// E-mail Address for Notification
        /// </summary>
        [StringLength(255)]
        public string Email
        {
            get => _email;
            set => this.Set(ref _email, value, Changed);
        }
    }
}
