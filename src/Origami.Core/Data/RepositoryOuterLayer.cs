using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryOuterLayer<T> : RepositoryLayer4Search<T>
        where T : class, IId, new()
    {
        protected RepositoryOuterLayer(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual void RefreshCache()
        {
            var k = KeyForCaching;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Refreshing cache for {k}");
            lock (OrigamiConstants.SyncRoot)
            {
                var l = ReadFromDatabase().ToList();
                MemoryCache.Set(k, l);
            }
        }

        public virtual Result<T> HTMLValidation(DataOperationContext<T> ctx)
        {
            if (ctx.Entity is BaseComment comment)
            {
                //avoiding cross-site script attacks
                if (comment.Content.Contains("<script", StringComparison.CurrentCultureIgnoreCase))
                {
                    return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
                }
                //avoiding cross-site script attacks
                if (comment.Content.Contains("<link", StringComparison.CurrentCultureIgnoreCase))
                {
                    return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
                }
                //avoiding cross-site script attacks
                if (comment.Content.Contains("<iframe", StringComparison.CurrentCultureIgnoreCase))
                {
                    return new(ctx.Entity) { ErrorMessage = Text.Original(Text.SomethingWentWrongPleaseTryAgain) };
                }
            }
            return new(ctx.Entity);
        }
    }
}
