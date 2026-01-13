using System.ComponentModel;

namespace Origami.Core.Models
{
    public interface ICommentChanged
    {
        /// <summary>
        /// Has the Comment Changed?
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> CommentChanged;
    }
}
