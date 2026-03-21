using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Table("oi_PhysicalPages")]
    [Index(nameof(Path), IsUnique = true, Name = "IX_oi_PhysicalPages_1")]
    public class OrigamiPhysicalPage :
        IId,
        IDateCreated,
        IChanged,
        IVersion,
        INew
    {
        private Content? _content;
        private DateTime _dateCreated;
        private Guid _id = Guid.NewGuid();
        private string _path = string.Empty;
        private byte[] _version = [];
        public OrigamiPhysicalPage() : base()
        {

        }

        public OrigamiPhysicalPage(Guid id) : base()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// Sometimes a physical page is tied to a page, post, video, etc.
        /// </summary>
        public Content? Content
        {
            get => _content;
            set => this.Set(ref _content, value, Changed);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => this.Set(ref _dateCreated, value, Changed);
        }

        [Key]
        public Guid Id
        {
            get => _id;
            set => this.Set(ref _id, value, Changed);
        }

        public bool New => Version.SequenceEqual([]);

        /// <summary>
        /// (Only, no scheme, host name/port, etc.) Path of the Physical Page
        /// </summary>
        [StringLength(1024)]
        public string Path
        {
            get => _path;
            set => this.Set(ref _path, value, Changed);
        }

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
