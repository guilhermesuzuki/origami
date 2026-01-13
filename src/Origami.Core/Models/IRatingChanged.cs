using System.ComponentModel;

namespace Origami.Core.Models
{
    public interface IRatingChanged
    {
        /// <summary>
        /// Has the entity changed? Has any of its important objects changed?
        /// </summary>
        event EventHandler<PropertyChangedEventArgs> RatingChanged;
    }
}
