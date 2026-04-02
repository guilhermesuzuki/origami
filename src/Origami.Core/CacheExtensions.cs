using AngleSharp.Dom;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Origami.Core
{
    public static class CacheExtensions
    {
        public static void Create<T>(this IMemoryCache memoryCache, IEnumerable<T> entities) where T : class
        {
            var key = typeof(T).KeyForCaching();

            lock (OrigamiConstants.SyncRoot)
            {
                var list = memoryCache.GetList<T>(key);
                if (list == null)
                {
                    memoryCache.Set(key, entities.ToList());
                    return;
                }
                list.AddRange(entities);
                memoryCache.Set(key, list);
            }
        }

        public static void Purge<T>(this IMemoryCache memoryCache, IEnumerable<T> entities) where T : class, IId
        {
            var key = typeof(T).KeyForCaching();

            lock (OrigamiConstants.SyncRoot)
            {
                var list = memoryCache.GetList<T>(key);
                if (list == null)
                {
                    return;
                }

                var found = from a in entities join b in list on a.Id equals b.Id select b;

                found.Each(list.Remove);
                memoryCache.Set(key, list);
            }
        }

        public static void Purge(this IMemoryCache memoryCache, OrigamiContent? entity)
        {
            if (entity != null)
            {
                memoryCache.Purge([entity]);
            }
        }

        public static void Save(this IMemoryCache memoryCache, OrigamiContent? entity)
        {
            memoryCache.Save<OrigamiContent>(entity);
        }

        public static void Save<T>(this IMemoryCache memoryCache, T? entity) where T : class, IId
        {
            if (entity == null) return;

            var key = typeof(T).KeyForCaching();

            lock (OrigamiConstants.SyncRoot)
            {
                var list = memoryCache.GetList<T>(key);
                if (list == null)
                {
                    memoryCache.Set(key, new List<T> { entity });
                    return;
                }

                list.RemoveAll(x => x.Id == entity.Id);
                list.Add(entity);

                memoryCache.Set(key, list);
            }
        }

        public static void Update<T>(this IMemoryCache memoryCache, IEnumerable<T> entities) where T : class, IId
        {
            var key = typeof(T).KeyForCaching();

            lock (OrigamiConstants.SyncRoot)
            {
                var list = memoryCache.GetList<T>(key);
                if (list == null)
                {
                    return;
                }

                var found = from a in entities
                            join b in list on a.Id equals b.Id
                            select b;

                found.Each(list.Remove);
                list.AddRange(entities);
                memoryCache.Set(key, list);
            }
        }
    }
}
