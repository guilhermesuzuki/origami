using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Origami.Core.Models
{
    public class OrigamiSoftwareRelease :
        OrigamiContent
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSoftwareRelease() : base()
        {
            this.Type = nameof(OrigamiSoftwareRelease);
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
            this.IsCommentEnabled = true;
            this.DateReleased = DateTime.UtcNow;
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiSoftwareRelease(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiSoftwareReleaseChanged = (sender, e) => { };
    }
}