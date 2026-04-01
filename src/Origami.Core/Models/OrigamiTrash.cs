using System.ComponentModel.DataAnnotations;

namespace Origami.Core.Models
{
    public class OrigamiTrash :
        BaseModel,
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
        IContent
    {
        public string Content { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DatePublished { get; set; }

        [Key]
        public Guid FakeId { get; set; }
        public override Guid Id { get => base.Id; set => base.Id = value; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool New => Version.SequenceEqual([]);
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public byte[] Version { get; set; } = [];
    }
}
