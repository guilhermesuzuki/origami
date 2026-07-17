using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Origami.Core.Models.Events
{
    public class AdminUserRestoresBlog : 
        OrigamiEvent,
        IChanged,
        IBlogId
    {
        protected Guid _blogId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public AdminUserRestoresBlog() : base()
        {
            this.Type = nameof(AdminUserRestoresBlog);
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }
    }
}
