using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PackageFiles")]
    [PrimaryKey(nameof(PackageId), nameof(FileOrder))]
    public class OrigamiPackageFile :
        IChanged
    {
        private int _fileOrder;
        private string _packageId = string.Empty;
        private string _filePath = string.Empty;

        private bool _isDirectory;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPackageFile() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Package Id
        /// </summary>
        [StringLength(128)]
        public string PackageId
        {
            get => _packageId;
            set => this.Set(ref _packageId, value, Changed);
        }

        /// <summary>
        /// File Order
        /// </summary>
        public int FileOrder
        {
            get => _fileOrder;
            set => this.Set(ref _fileOrder, value, Changed);
        }

        /// <summary>
        /// File Path
        /// </summary>
        [StringLength(255)]
        public string FilePath
        {
            get => _filePath;
            set => this.Set(ref _filePath, value, Changed);
        }

        /// <summary>
        /// Is Directory?
        /// </summary>
        public bool IsDirectory
        {
            get => _isDirectory;
            set => this.Set(ref _isDirectory, value, Changed);
        }
    }
}
