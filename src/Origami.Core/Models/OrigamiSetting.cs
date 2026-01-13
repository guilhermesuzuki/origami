using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Settings")]
    public class OrigamiSetting :
        ISettingChanged,
        IId,
        IName
    {
        private Guid _id = Guid.NewGuid();
        protected string _name = string.Empty;
        protected string _value = string.Empty;

        public OrigamiSetting() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> SettingChanged = (sender, e) => { };

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, SettingChanged);
        }

        /// <summary>
        /// Setting Name
        /// </summary>
        [StringLength(50)]
        public string Name
        {
            get => _name;
            set => this.Set(ref _name, value, SettingChanged);
        }

        /// <summary>
        /// Setting Value [nvarchar(max)]
        /// </summary>
        public string Value
        {
            get => _value;
            set => this.Set(ref _value, value, SettingChanged);
        }
    }
}
