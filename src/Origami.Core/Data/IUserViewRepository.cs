using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IUserViewRepository : IRepository<OrigamiUserView>
    {
        Task<IEnumerable<ProcessedUserView>> GetBrowsersAsync(Guid blog, DateTime start, DateTime end);
        Task<IEnumerable<ProcessedUserViewForHistory>> GetHistoryAsync(TimePeriod timePeriod, Guid blog, DateTime start, DateTime end);
        Task<IEnumerable<ProcessedUserView>> GetPlatformsAsync(Guid blog, DateTime start, DateTime end);
    }
}
