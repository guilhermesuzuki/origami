using System.ComponentModel;
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
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
            this.IsCommentEnabled = true;
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiPostChanged = delegate { };
    }
}