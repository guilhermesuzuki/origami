using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserActivityRepository :
        RepositoryOuterLayer<OrigamiUserActivity>,
        IUserActivityRepository
    {
        public UserActivityRepository(
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {

        }

        public override Result<OrigamiUserActivity> Create(DataOperationContext<OrigamiUserActivity> entity)
        {
            throw new NotImplementedException();
        }

        public override Result<OrigamiUserActivity> Delete(DataOperationContext<OrigamiUserActivity> entity)
        {
            throw new NotImplementedException();
        }

        public override Result<OrigamiUserActivity> Update(DataOperationContext<OrigamiUserActivity> entity)
        {
            throw new NotImplementedException();
        }
    }
}
