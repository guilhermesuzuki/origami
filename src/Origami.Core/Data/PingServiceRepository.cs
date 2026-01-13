using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PingServiceRepository : RepositoryOuterLayer<OrigamiPingService>, IPingServiceRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PingServiceRepository(
            IMemoryCache memoryCache,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }
    }
}
