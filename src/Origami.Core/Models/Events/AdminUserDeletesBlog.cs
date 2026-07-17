using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Origami.Core.Models.Events
{
    public class AdminUserDeletesBlog : 
        OrigamiEvent,
        IChanged,
        IBlogId
    {
        protected Guid _blogId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, p) => { };

        public AdminUserDeletesBlog() : base()
        {
            this.Type = nameof(AdminUserDeletesBlog);
        }

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }
    }
}
