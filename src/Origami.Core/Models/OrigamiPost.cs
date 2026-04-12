using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    public class OrigamiPost : OrigamiContent
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPost() : base()
        {
            Type = nameof(OrigamiPost);
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiPostChanged = delegate { };
    }
}