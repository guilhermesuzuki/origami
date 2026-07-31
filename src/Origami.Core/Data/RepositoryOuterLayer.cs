using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace Origami.Core.Data
{
    public abstract class RepositoryOuterLayer<T> : RepositoryLayer4Search<T>
        where T : class, IId
    {
        protected RepositoryOuterLayer(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath webRootPath,
            IAppFacade appFacade)
            : base(appFacade, dbContextFactory, memoryCache, webRootPath, text)
        {
            
        }

        public virtual void RefreshCache()
        {
            var k = KeyForCaching;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Refreshing cache for {k}");
            lock (OrigamiConstants.SyncRoot)
            {
                using var db = DbContextFactory.CreateDbContext();

                //front-end
                if (_appFacade.Admin.GetValueOrDefault() == false)
                {
                    if (typeof(T).IsAssignableTo(typeof(OrigamiBlog)) == true)
                    {
                        var blogs = from a in db.Blogs.AsNoTracking()
                                    where a.IsDeleted == false
                                    where a.IsActive == true
                                    select a;

                        MemoryCache.Set(k, blogs.ToList());
                        return;
                    }
                    if (typeof(T).IsAssignableTo(typeof(OrigamiContent)) == true)
                    {
                        var contents = from a in db.Contents.AsNoTracking()
                                       join b in db.Blogs.AsNoTracking() on a.BlogId equals b.Id into blogs
                                       from blog in blogs.DefaultIfEmpty()  
                                       where a.IsDeleted == false
                                       where a.IsPublished == true
                                       where a.DatePublished <= DateTime.UtcNow
                                       where (blog == null || blog.IsDeleted == false && blog.IsActive == true)
                                       select a;

                        MemoryCache.Set(k, contents.ToList());
                        return;
                    }
                }

                var l = db.Read<T>();
                MemoryCache.Set(k, l);
            }
        }
    }
}
