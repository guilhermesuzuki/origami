using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public class OrigamiUserTrash :
        IId,
        IType,
        IFakeId,
        IBlogId,
        IDateCreated,
        IDateModified,
        IPublished,
        IDeleted,
        IVersion,
        INew,
        ITitle,
        IName,
        IContent
    {
        public Guid BlogId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DatePublished { get; set; }

        [Key]
        public Guid FakeId { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public bool New => Version.SequenceEqual([]);
        public string Type { get; set; } = string.Empty;
        public byte[] Version { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
