using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_VideoNotifications")]
    [Index(nameof(VideoId), nameof(Email), IsUnique = true, Name = "IX_oi_VideoNotifications_1")]
    public class OrigamiVideoNotification :
        IChanged,
        IId,
        IFKVideo,
        IEmail
    {
        private Guid _id;
        private Guid _videoId;
        private OrigamiVideo? _video;

        private string _email = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiVideoNotification() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public Guid VideoId
        {
            get => _videoId;
            set => this.Set(ref _videoId, value, Changed);
        }

        [ForeignKey(nameof(VideoId))]
        public OrigamiVideo? Video
        {
            get => _video;
            set => this.Set(ref _video, value, Changed);
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
