using System.ComponentModel;
using System.Globalization;

namespace Origami.Core.Models
{
    public class OrigamiSpecialPage : OrigamiContent
    {
        /// <summary>
        /// Default maintenance page
        /// </summary>
        public readonly static OrigamiSpecialPage Maintenance = new()
        {
            Id = Guid.Parse("7B66B400-69CC-4974-BE2F-BDA3F45DD38C"),
            Content = "This website is under maintenance.",
            LanguageWrittenOn = "en-US",
            Title = "Maintenance page",
            Type = OrigamiSpecialPageTypes.Maintenance.ToString(),
        };

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSpecialPage() : base()
        {
            this.Type = nameof(OrigamiSpecialPage);
            this.Subtype = OrigamiSpecialPageTypes.CookiePolicy.ToString();
            this.LanguageWrittenOn = CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";
        }

        /// <summary>
        /// Id constructor
        /// </summary>
        /// <param name="id"></param>
        public OrigamiSpecialPage(Guid id) : this()
        {
            Id = id;
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiSpecialPageChanged = delegate { };

        /// <summary>
        /// Special messages are not attached to a particular blog
        /// </summary>
        public override Guid? BlogId { get => null; set { } }
    }
}
