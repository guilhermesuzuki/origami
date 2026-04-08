using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    public class OrigamiQuickNote : OrigamiContent
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public OrigamiQuickNote() : base()
        {
            Type = nameof(OrigamiQuickNote);
            IsDraft = false;
        }

        [NotMapped]
        public string Background
        {
            get { return this.Get().Background; }
            set { this.Set(x => x.Background = value); }
        }
    }
}
