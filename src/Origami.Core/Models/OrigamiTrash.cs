using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public class OrigamiTrash :
        IId,
        IType,
        IFakeId,
        IDateCreated,
        IDateModified,
        IPublished,
        IDeleted,
        IVersion,
        INew,
        ITitle,
        IName,
        IContent,
        INanoId
    {
        public string Content { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DatePublished { get; set; }

        [Key]
        public Guid FakeId { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NanoId { get; set; } = string.Empty;
        public bool New => Version.SequenceEqual([]);
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public byte[] Version { get; set; } = [];
    }
}
