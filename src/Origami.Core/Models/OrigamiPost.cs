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

        /// <summary>
        /// Fake post
        /// </summary>
        public static OrigamiPost GetFake() => new() { Id = Guid.Empty, Title = "Veritas et Sapientia: De Vita et Cogitationibus" };

        /// <summary>
        /// Fake posts
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiPost> GetFakes(int count = 6)
        {
            for (int i = 0; i < count; i++) yield return GetFake();
        }
    }
}