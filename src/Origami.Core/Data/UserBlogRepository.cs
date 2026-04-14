using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserBlogRepository :
        RepositoryOuterLayer<OrigamiUserBlog>,
        IUserBlogRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public UserBlogRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }
    }
}
