using System.ComponentModel;

namespace Origami.Core.Models
{
    public interface IContentChanged
    {
        /// <summary>
        /// Has the entity changed? Has any of its important objects changed?
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> ContentChanged;
    }
}
