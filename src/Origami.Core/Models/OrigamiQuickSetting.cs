using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_QuickSettings")]
    public class OrigamiQuickSetting :
        BaseSetting,
        ISettingChanged,
        IId
    {
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiQuickSetting() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> SettingChanged = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get { return _id; }
            set { this.Set(ref _id, value, SettingChanged); }
        }

        [StringLength(255)]
        public override string Name
        {
            get { return _name; }
            set { this.Set(ref _name, value, SettingChanged); }
        }

        [StringLength(255)]
        public override string Value
        {
            get { return _value; }
            set { this.Set(ref _name, value, SettingChanged); }
        }
    }
}
