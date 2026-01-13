using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_Packages")]
    public class OrigamiPackage :
        IChanged
    {
        private string _packageId = string.Empty;
        private string _version = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPackage() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Package Id
        /// </summary>
        [Key]
        [StringLength(128)]
        public string PackageId
        {
            get => _packageId;
            set => this.Set(ref _packageId, value, Changed);
        }

        /// <summary>
        /// Package Version
        /// </summary>
        [StringLength(128)]
        public string Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
