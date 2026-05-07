using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class DashboardRepository :
        RepositoryOuterLayer<Dashboard>,
        IDashboardRepository
    {
        public DashboardRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewDashboard);
    }
}
