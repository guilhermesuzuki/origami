using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System.Diagnostics;

namespace Origami.Core.Data
{
    public class MyMemoryCache(IMemoryCache memoryCache, IDbContextFactory<OrigamiDbContext> dbContextFactory) : IMyMemoryCache
    {
        public IEnumerable<object> Keys
        {
            get
            {
                if (memoryCache is MemoryCache memCache)
                {
                    return memCache.Keys;
                }
                throw new InvalidOperationException("Underlying cache is not a MemoryCache.");
            }
        }

        public ICacheEntry CreateEntry(object key)
        {
            return memoryCache.CreateEntry(key);
        }

        public void Dispose()
        {
            memoryCache.Dispose();
        }

        public void Remove(object key)
        {
            memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object? value)
        {
            return memoryCache.TryGetValue(key, out value);
        }

        public List<T> Read<T>() where T : class
        {
            var timestamp = Stopwatch.GetTimestamp();
            var key = typeof(T).KeyForCaching();

            if (typeof(T).IsAbstract == false)
            {
                var t = Activator.CreateInstance<T>();
                switch (t)
                {
                    case OrigamiPage:
                    case OrigamiPost:
                    case OrigamiSpecialMessage:
                    case OrigamiSpecialPage:
                    case OrigamiVideo:
                    case OrigamiQuickNote:
                        return this.Read<OrigamiContent>().OfType<T>().ToList();
                    default: break;
                }
            }

            try
            {
                //race condition
                if (memoryCache.Get(key) == null)
                {
                    lock (OrigamiConstants.SyncRoot)
                    {
                        if (memoryCache.Get(key) == null)
                        {
                            using var db = dbContextFactory.CreateDbContext();
                            memoryCache.Set(key, db.Read<T>());
                        }
                    }
                }

                return memoryCache.GetList<T>(key) ?? [];
            }
            finally
            {
                var elapsedTime = Stopwatch.GetElapsedTime(timestamp);
                Console.ForegroundColor = elapsedTime.Milliseconds >= 100 ? ConsoleColor.Red : ConsoleColor.White;
                Console.WriteLine($"{key} obtained in {elapsedTime}");
            }
        }
    }
}
