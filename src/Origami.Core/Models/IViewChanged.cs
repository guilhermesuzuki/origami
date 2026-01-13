using System.ComponentModel;

namespace Origami.Core.Models
{
    public interface IViewChanged
    {
        /// <summary>
        /// Has the View Changed?
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> ViewChanged;
    }
}
