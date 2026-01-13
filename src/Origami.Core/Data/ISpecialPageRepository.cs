using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ISpecialPageRepository : IRepository<OrigamiSpecialPage>, IPublish<OrigamiSpecialPage>
    {
        Result EnterMaintenanceMode(DataOperationContext context);
        Result LeaveMaintenanceMode(DataOperationContext context);
    }
}
