using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Origami.Core.Models
{
    [Table("oi_UserBlogs")]
    public class OrigamiUserBlog : BaseModel,
        IChanged,
        IBlogId
    {
        protected Guid _blogId;
        protected Guid _userId;

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        public Guid BlogId
        {
            get => _blogId;
            set => this.Set(ref _blogId, value, Changed);
        }

        public Guid UserId
        {
            get => _userId;
            set => this.Set(ref _userId, value, Changed);
        }
    }
}
