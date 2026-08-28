using System.ComponentModel;

namespace Origami.Core.Models
{
    public class OrigamiPage : OrigamiContent
    {
        protected bool _isFrontPage;
        protected string _keywords = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiPage() : base()
        {
            this.Type = nameof(OrigamiPage);
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiPageChanged = delegate { };

        /// <summary>
        /// Is this Page the Front Page?
        /// </summary>
        public bool IsFrontPage
        {
            get => _isFrontPage;
            set => this.Set(ref _isFrontPage, value, OrigamiPageChanged);
        }

        /// <summary>
        /// Page Keywords (nvarchar[max])
        /// </summary>
        public string Keywords
        {
            get => _keywords;
            set => this.Set(ref _keywords, value, OrigamiPageChanged);
        }

        /// <summary>
        /// Fake page
        /// </summary>
        public static OrigamiPage GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake pages
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiPage> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }
    }
}
