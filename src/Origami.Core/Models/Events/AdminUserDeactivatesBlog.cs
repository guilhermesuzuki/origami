using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Origami.Core.Models.Events
{
    public class AdminUserDeactivatesBlog : 
        OrigamiEvent,
        IChanged,
        IBlogId
    {
        protected Guid _blogId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public AdminUserDeactivatesBlog() : base()
        {
            this.Type = nameof(AdminUserDeactivatesBlog);
        }

        [Column(nameof(BlogId))]
        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }
    }
}
