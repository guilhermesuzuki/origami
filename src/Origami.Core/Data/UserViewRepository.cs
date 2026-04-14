using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserViewRepository :
        RepositoryOuterLayer<OrigamiUserView>,
        IUserViewRepository
    {
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public UserViewRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public async Task<IEnumerable<ProcessedUserView>> GetBrowsersAsync(Guid blog, DateTime start, DateTime end)
        {
            using var db = await DbContextFactory.CreateDbContextAsync();

            var paramBlog = new SqlParameter("@blog", blog);
            var paramStart = new SqlParameter("@start", start);
            var paramEnd = new SqlParameter("@end", end);

            return await db.ProcessedUserViews
                .FromSqlRaw($"EXEC dbo.usp_GetBrowserHistory @blog, @start, @end", paramBlog, paramStart, paramEnd)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessedUserViewForHistory>> GetHistoryAsync(TimePeriod timePeriod, Guid blog, DateTime start, DateTime end)
        {
            const string timePeriod24hours = "dd [HH]";

            var format = timePeriod switch
            {
                TimePeriod.Last24Hours => timePeriod24hours,
                TimePeriod.Last7Days => "yyyy-MM-dd",
                TimePeriod.Last30Days => "yyyy-MM-dd",
                TimePeriod.Last90Days => "yyyy-MM",
                TimePeriod.Last180Days => "yyyy-MM",
                TimePeriod.Last365Days => "yyyy-MM",
                TimePeriod.CurrentMonth => "yyyy-MM-dd",
                TimePeriod.CurrentYear => "yyyy",
                TimePeriod.Everything => "yyyy",
                _ => "yyyy",
            };

            // Adjust for English culture in 24 hours format
            if (format == timePeriod24hours && Thread.CurrentThread.CurrentUICulture.En() == true)
            {
                format = "dd [hh tt]";
            }

            using var db = await DbContextFactory.CreateDbContextAsync();

            var paramBlog = new SqlParameter("@blog", blog);
            var paramStart = new SqlParameter("@start", start);
            var paramEnd = new SqlParameter("@end", end);
            var paramFormat = new SqlParameter("@format", format);

            return await db.ProcessedUserViewForHistories
                .FromSqlRaw($"EXEC dbo.usp_GetHistoryByFormat @blog, @start, @end, @format", paramBlog, paramStart, paramEnd, paramFormat)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcessedUserView>> GetPlatformsAsync(Guid blog, DateTime start, DateTime end)
        {
            using var db = await DbContextFactory.CreateDbContextAsync();

            var paramBlog = new SqlParameter("@blog", blog);
            var paramStart = new SqlParameter("@start", start);
            var paramEnd = new SqlParameter("@end", end);

            return await db.ProcessedUserViews
                .FromSqlRaw($"EXEC dbo.usp_GetPlatformHistory @blog, @start, @end", paramBlog, paramStart, paramEnd)
                .ToListAsync();
        }
    }
}
