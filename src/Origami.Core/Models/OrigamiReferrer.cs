using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Referrers")]
    public class OrigamiReferrer :
        IChanged
    {
        private int _referrerRowId;
        private Guid _blogId;
        private Guid _referrerId;
        private DateTime _referralDay;
        private string _referrerUrl = string.Empty;
        private int _referralCount;
        private string _url = string.Empty;
        private bool? _isSpam;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiReferrer() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        [Key]
        public int ReferrerRowId
        {
            get { return _referrerRowId; }
            set { this.Set(ref _referrerRowId, value, Changed); }
        }

        public Guid BlogId
        {
            get { return _blogId; }
            set { this.Set(ref _blogId, value, Changed); }
        }

        public Guid ReferrerId
        {
            get { return _referrerId; }
            set { this.Set(ref _referrerId, value, Changed); }
        }

        public DateTime ReferralDay
        {
            get { return _referralDay; }
            set { this.Set(ref _referralDay, value, Changed); }
        }

        [StringLength(255)]
        public string ReferrerUrl
        {
            get { return _referrerUrl; }
            set { this.Set(ref _referrerUrl, value, Changed); }
        }

        public int ReferralCount
        {
            get { return _referralCount; }
            set { this.Set(ref _referralCount, value, Changed); }
        }

        [StringLength(255)]
        public string Url
        {
            get { return _url; }
            set { this.Set(ref _url, value, Changed); }
        }

        public bool? IsSpam
        {
            get { return _isSpam; }
            set { this.Set(ref _isSpam, value, Changed); }
        }
    }
}
