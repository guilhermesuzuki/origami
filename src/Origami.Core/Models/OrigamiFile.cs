using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    public class OrigamiFile :
        IId,
        IChanged,
        IDateCreated,
        IDateModified,
        ILocalPath,
        IWebPath
    {
        private Guid _id;
        private DateTime _dateCreated;
        private DateTime? _dateModified;
        private string? _language;
        private string _localPath = string.Empty;
        private string _webPath = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiFile() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Identifier for this file
        /// </summary>
        [NotMapped]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        /// <summary>
        /// Date/Time the file was created
        /// </summary>
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        /// <summary>
        /// Date/Time the file was modified
        /// </summary>
        public DateTime? DateModified
        {
            get => _dateModified;
            set => this.Set(ref _dateModified, value, Changed);
        }

        /// <summary>
        /// Only the filename is returned from the <see cref="LocalPath"/>
        /// </summary>
        [NotMapped]
        public string FileName
        {
            get
            {
                return _webPath.Has() == true ? Path.GetFileName(_webPath) : string.Empty;
            }
        }

        /// <summary>
        /// File Language (if any)
        /// </summary>
        [StringLength(10)]
        public string? Language
        {
            get => _language;
            set => this.Set(ref _language, value, Changed);
        }

        [StringLength(255)]
        public string LocalPath
        {
            get => _localPath;
            set => this.Set(ref _localPath, value, Changed);
        }

        [StringLength(255)]
        public string WebPath
        {
            get => _webPath;
            set => this.Set(ref _webPath, value, Changed);
        }
    }
}
