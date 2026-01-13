using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    /// <summary>
    /// Page, Post or Video, etc.
    /// </summary>
    [PrimaryKey(nameof(FakeId))]
    public class OrigamiUserContent :
        BaseContent,
        IContentChanged,
        IId,
        IType,
        IFakeId
    {
        private string _type = string.Empty;

        public event EventHandler<PropertyChangedEventArgs> ContentChanged = (sender, e) => { };

        /// <summary>
        /// This is not mapped
        /// </summary>
        [NotMapped]
        public override string? AdditionalInfo { get => base.AdditionalInfo; set => base.AdditionalInfo = value; }

        /// <summary>
        /// This is not mapped
        /// </summary>
        [NotMapped]
        public override string Content { get => base.Content; set => base.Content = value; }

        /// <summary>
        /// This is not mapped
        /// </summary>
        [NotMapped]
        public override string? Description { get => base.Description; set => base.Description = value; }

        public Guid FakeId { get; set; }

        /// <summary>
        /// This is not mapped
        /// </summary>
        [NotMapped]
        public override bool IsCommentEnabled
        {
            get => _isCommentEnabled;
            set => this.Set(ref _isCommentEnabled, value, ContentChanged);
        }

        /// <summary>
        /// User content type
        /// </summary>
        public string Type
        {
            get => _type;
            set => this.Set(ref _type, value, ContentChanged);
        }
    }
}
