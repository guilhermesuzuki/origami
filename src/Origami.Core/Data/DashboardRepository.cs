using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class DashboardRepository :
        RepositoryOuterLayer<Dashboard>,
        IDashboardRepository
    {
        public DashboardRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ViewDashboard);
    }
}
