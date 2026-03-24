using NanoidDotNet;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    public class OrigamiSpecialMessage : OrigamiContent
    {
        protected DateOnly _endDate = DateOnly.MaxValue;
        protected DateOnly _startDate = DateOnly.MinValue;

        public event EventHandler<PropertyChangedEventArgs> OrigamiSpecialMessageChanged = (sender, e) => { };

        public OrigamiSpecialMessage() : base()
        {
            this.Type = nameof(OrigamiSpecialMessage);
        }

        public DateOnly EndDate
        {
            get => _endDate;
            set => this.Set(ref _endDate, value, OrigamiSpecialMessageChanged);
        }

        public DateOnly StartDate
        {
            get => _startDate;
            set => this.Set(ref _startDate, value, OrigamiSpecialMessageChanged);
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

        /// <summary>
        /// MudBlazor compatible End Date
        /// </summary>
        [NotMapped]
        public DateTime? MB_EndDate
        {
            get => EndDate == DateOnly.MaxValue ? null : EndDate.ToDateTime(new TimeOnly(0, 0));
            set => EndDate = value.HasValue ? DateOnly.FromDateTime(value.Value) : DateOnly.MaxValue;
        }
    }
}
