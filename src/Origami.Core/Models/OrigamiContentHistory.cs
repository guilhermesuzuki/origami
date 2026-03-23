using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Origami.Core.Models
{
    [Table("oi_ContentHistories")]
    public class OrigamiContentHistory :
        IChanged,
        IId,
        IContentId,
        IVersion,
        IDateCreated,
        IAuthorId,
        INew
    {
        protected Guid _authorId;
        protected Guid _contentId;
        protected DateTime _dateCreated = DateTime.UtcNow;
        protected Guid _id;
        protected byte[] _version = Array.Empty<byte>();

        public event EventHandler<PropertyChangedEventArgs> Changed = delegate { };

        public Guid AuthorId
        {
            get => _authorId;
            set => this.Set(ref _authorId, value, Changed);
        }

        public Guid ContentId
        {
            get => _contentId;
            set => this.Set(ref _contentId, value, Changed);
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

        public bool New => this.Version.SequenceEqual(Array.Empty<byte>());

        [Timestamp]
        public byte[] Version
        {
            get => _version;
            set => this.Set(ref _version, value, Changed);
        }
    }
}
