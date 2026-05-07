using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    public class OrigamiSpecialMessage : OrigamiContent
    {
        protected DateOnly _endDate = DateOnly.MaxValue;
        protected DateOnly _startDate = DateOnly.MinValue;

        public OrigamiSpecialMessage() : base()
        {
            this.Type = nameof(OrigamiSpecialMessage);
        }

        public event EventHandler<PropertyChangedEventArgs> OrigamiSpecialMessageChanged = (sender, e) => { };

        /// <summary>
        /// Special messages are not attached to a particular blog
        /// </summary>
        public override Guid? BlogId { get => null; set { } }

        public DateOnly EndDate
        {
            get => _endDate;
            set => this.Set(ref _endDate, value, OrigamiSpecialMessageChanged);
        }

        /// <summary>
        /// MudBlazor compatible End Date
        /// </summary>
        [NotMapped]
        public DateTime? MB_EndDate
        {
            get => EndDate == DateOnly.MaxValue ? null : EndDate.ToDateTime(new TimeOnly(0, 0));
            set => EndDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : DateOnly.MaxValue;
        }

        /// <summary>
        /// MudBlazor compatible Start Date
        /// </summary>
        [NotMapped]
        public DateTime? MB_StartDate
        {
            get => StartDate == DateOnly.MinValue ? null : StartDate.ToDateTime(new TimeOnly(0, 0));
            set => StartDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : DateOnly.MinValue;
        }

        public DateOnly StartDate
        {
            get => _startDate;
            set => this.Set(ref _startDate, value, OrigamiSpecialMessageChanged);
        }
    }
}
