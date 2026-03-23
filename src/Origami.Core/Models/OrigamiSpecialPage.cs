using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        protected string _subtype = OrigamiSpecialPageTypes.CookiePolicy.ToString();

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiSpecialPage() : base()
        {
            this.Type = nameof(OrigamiSpecialPage);
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

        [StringLength(64)]
        public string SubType
        {
            get => _subtype;
            set => this.Set(ref _subtype, value, OrigamiSpecialPageChanged);
        }

        /// <summary>
        /// Fake page
        /// </summary>
        public static OrigamiSpecialPage GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake pages
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiSpecialPage> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }
    }
}
