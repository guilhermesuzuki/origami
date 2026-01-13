namespace Origami.Core.Models
{
    /// <summary>
    /// List for time periods, used in the Dashboard admin page (user view history, charts, etc.)
    /// </summary>
    public enum TimePeriod : byte
    {
        Last24Hours = 0,
        Last7Days = 1,
        Last30Days = 2,
        Last90Days = 3,
        Last180Days = 4,
        Last365Days = 5,
        CurrentMonth = 6,
        CurrentYear = 7,
        Everything = 8,
    }
}
