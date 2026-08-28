using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    public class OrigamiUserActivity :
        IChanged,
        IId,
        IFakeId
    {
        private DateTime _date;
        private Guid _id = Guid.NewGuid();
        private Guid? _socialProfileId;
        private Guid? _userId;
        private string _type = string.Empty;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Date and Time when this activity happened
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => this.Set(ref _date, value, Changed);
        }

        [Key]
        public Guid FakeId { get; set; }

        /// <summary>
        /// Id (Post Comment, Post Comment Reaction, Video Comment, Video Comment Reaction, etc.)
        /// </summary>
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// Social Profile Id
        /// </summary>
        public Guid? SocialProfileId
        {
            get => _socialProfileId;
            set => this.Set(ref _socialProfileId, value, Changed);
        }

        /// <summary>
        /// User Id
        /// </summary>
        public Guid? UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, Changed);
        }

        /// <summary>
        /// Post Comment, Post Comment Reaction, Video Comment, Video Comment Reaction, etc.
        /// </summary>
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, Changed);
        }
    }
}
