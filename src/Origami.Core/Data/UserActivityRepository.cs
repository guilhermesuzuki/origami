using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserActivityRepository :
        RepositoryOuterLayer<OrigamiUserActivity>,
        IUserActivityRepository
    {
        public UserActivityRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
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
