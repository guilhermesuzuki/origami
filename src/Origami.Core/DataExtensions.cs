using CloneExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Origami.Core
{
    public static class DataExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blogs"></param>
        /// <returns></returns>
        public static IEnumerable<OrigamiBlog> Active(this IEnumerable<OrigamiBlog> blogs)
        {
            if (blogs != null)
            {
                return blogs.Where(x => x.IsActive).OrderBy(x => x.IsPrimary ? 0 : 1).ThenBy(x => x.Name);
            }
            return [];
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        public static IEnumerable<T> Author<T>(this IEnumerable<T> entities, IId author)
            where T : IAuthorId
        {
            return entities.Where(x => x.AuthorId == author.Id);
        }

        public static void CreateCache<T>(this IMemoryCache memoryCache, T entity)
                    where T : class
        {
            var key = typeof(T).KeyForCaching();
            lock (OrigamiConstants.SyncRoot)
            {
                var list = memoryCache.GetList<T>(key) ?? throw new InvalidOperationException("Cache list not found");
                list.Add(entity);
                memoryCache.Set(key, list);
            }
        }

        /// <summary>
        /// Blogs default sorting/ordering
        /// </summary>
        /// <param name="blogs">blogs to be sorted in a default manner</param>
        /// <returns></returns>
        public static List<OrigamiBlog> DefaultSorting(this IEnumerable<OrigamiBlog> blogs)
        {
            var standard = new List<OrigamiBlog>();
            standard.AddRange(blogs.Where(x => x.IsPrimary));
            standard.AddRange(blogs.Where(x => x.IsPrimary == false).OrderBy(x => x.Name));
            return standard.Each(x => x.Order = null).ToList().GetClone();
        }

        public static IEnumerable<T> Drafts<T>(this IEnumerable<T> entities)
            where T : IDraft
        {
            return entities.Where(x => x.IsDraft.GetValueOrDefault() == true).NonDeleted();
        }

        /// <summary>
        /// Gets the front page for a blog
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public static OrigamiPage? FrontPage(this IEnumerable<OrigamiPage> pages, Guid blogId)
        {
            var frontpage = pages.NonDeleted().Published().Blog(blogId).Where(x => x.IsFrontPage).FirstOrDefault();
            if (frontpage != null)
            {
                var lang = CultureInfo.CurrentUICulture.Name.Split('-').First();

                var translated = from p in pages.NonDeleted().Published()
                                 where p.BlogId == blogId
                                 where p.ParentId == frontpage.Id
                                 where p.LanguageWrittenOn.StartsWith(lang)
                                 orderby p.LanguageWrittenOn.Like(CultureInfo.CurrentUICulture.Name) ? 0 : 1, p.LanguageWrittenOn
                                 select p;

                return translated.FirstOrDefault() ?? frontpage;
            }
            return null;
        }

        public static string[] GenerateTOTPRecoveryCodes(this IUserRepository userRepository)
        {
            var codes = new List<string>();
            while (codes.Distinct().Count() < 10)
            {
                codes.Add(NanoidDotNet.Nanoid.Generate(NanoidDotNet.Nanoid.Alphabets.Digits, 6));
            }
            return [.. codes.Distinct().Take(10).OrderBy(x => x)];
        }

        public static IEnumerable<T> GetAllChildren<T>(this IEnumerable<T>? source, T entity)
                    where T : class, IId
        {
            return source.GetAllChildren([entity]);
        }

        public static IEnumerable<T> GetAllChildren<T>(this IEnumerable<T>? source, IEnumerable<T> entities)
            where T : class, IId
        {
            if (source == null) return [];
            if (typeof(T).Implements<IParentIdNull>() == false) return [];

            var list = new List<T>();
            foreach (var entity in entities)
            {
                var children = source.Cast<IParentIdNull>().Where(x => x.ParentId == entity.Id).Cast<T>().ToList();
                foreach (var child in children)
                {
                    list.AddRange(source.GetAllChildren(child));
                    list.Add(child);
                }
            }

            return list;
        }

        public static IEnumerable<Guid> GetAllChildren<T>(this IEnumerable<T>? source, Guid id)
            where T : class, IId
        {
            if (source == null) return [];

            var list = new List<Guid>();
            var children = source.Cast<IParentIdNull>().Where(x => x.ParentId == id).Cast<T>().Select(x => x.Id).ToList();
            foreach (var child in children)
            {
                list.AddRange(source.GetAllChildren(child));
                list.Add(child);
            }

            return list;
        }

        /// <summary>
        /// Privacy policy (for the current language)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<OrigamiSpecialPage> GetBySubType(this IEnumerable<OrigamiSpecialPage> pages, OrigamiSpecialPageTypes type)
        {
            return pages
                .NonDeleted()
                .Published()
                .Where(x => x.Subtype == type.ToString())
                .ToList()
                .OrderBy(x => x.LanguageWrittenOn.Like(CultureInfo.CurrentUICulture.Name) == true ? 0 : 1)
                .ThenBy(x => x.LanguageWrittenOn.StartsWith(_getLanguage()) == true ? 2 : 3)
                .ThenBy(x => x.LanguageWrittenOn);
        }

        /// <summary>
        /// Retrieves the child entities of a given parent entity from a collection of entities.
        /// </summary>
        /// <typeparam name="T">The type of the entities in the collection.</typeparam>
        /// <typeparam name="T2">The type of the parent entity, which must implement <see cref="IParentIdNull{T}"/>, <see cref="T"/>, and <see cref="IId"/>.</typeparam>
        /// <param name="entities">The collection of entities to search for children.</param>
        /// <param name="entity">The parent entity whose children are to be retrieved.</param>
        /// <returns>A collection of child entities of the given parent entity.</returns>
        public static IEnumerable<T> GetChildren<T, T2>(this IEnumerable<T> entities, T2 entity)
            where T2 : IParentIdNull, T, IId
        {
            return [.. entities.Cast<T2>().Where(x => x.ParentId == entity.Id).Cast<T>()];
        }

        /// <summary>
        /// Generates a sequence of <see cref="DataOperationContext{T}"/> objects for each entity in the specified
        /// collection.
        /// </summary>
        /// <typeparam name="T">The type of the entities in the collection.</typeparam>
        /// <param name="entities">The collection of entities for which to create ctxs. Cannot be null.</param>
        /// <param name="simple">The base ctx containing user and date-time information to be applied to each entity.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DataOperationContext{T}"/> objects, each associated with an
        /// entity from the input collection.</returns>
        public static IEnumerable<DataOperationContext<T>> GetContexts<T>(this IEnumerable<T> entities, DataOperationContext simple)
        {
            foreach (var entity in entities)
            {
                yield return new(simple.User, simple.DateTime, entity);
            }
        }

        /// <summary>
        /// Filters and orders a collection of blogs for front-end display.
        /// </summary>
        /// <param name="blogs">The collection of blogs to filter and order.</param>
        /// <returns>A list of blogs that are not deleted and are active, ordered by custom order if available, otherwise by primary status and name.</returns>
        public static List<OrigamiBlog> GetFrontEnd(this IEnumerable<OrigamiBlog> blogs)
        {
            blogs = blogs.NonDeleted().Active();

            //has custom order
            if (blogs.Any(x => x.Order.HasValue == true))
            {
                var custom = new List<OrigamiBlog>();

                custom.AddRange(blogs.Where(x => x.Order.HasValue).OrderBy(x => x.Order));
                custom.AddRange(blogs.Where(x => x.Order.HasValue == false).OrderBy(x => x.Name));

                return custom.GetClone();
            }

            return blogs.DefaultSorting();
        }

        /// <summary>
        /// Gets a list instance of <typeparamref name="T"/> from Cache (given a key)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="distributedCache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T>? GetList<T>(this IMemoryCache memoryCache, string key)
            where T : class
        {
            return memoryCache.TryGetValue(key, out List<T>? value) == true ? value : null;
        }

        /// <summary>
        /// Gets the Origami connection string
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetOrigamiConnectionString(this IConfiguration configuration)
        {
            var words = new List<string> { "origami", "Origami", "oriGami", "ORIGAMI", };

            foreach (var origami in words)
            {
                var connection = configuration.GetConnectionString(origami);
                if (connection != null) return connection;
            }

            throw new Exception("The Origami connection string does NOT exist in the appsettings file");
        }

        public static List<OrigamiRole> GetRolesFromDatabase(this DbContext db)
        {
            var roles = db.Set<OrigamiRole>().AsNoTracking().ToList();

            foreach (var role in roles)
            {
                var rightRoles = db.Set<OrigamiRightRole>().AsNoTracking().Where(x => x.RoleId == role.Id).ToList();

                var match = from property in role.GetType().GetProperties()
                            join rt in db.Set<OrigamiRight>().AsNoTracking() on property.Name equals rt.Name
                            join rr in rightRoles on rt.Id equals rr.RightId
                            where property.CanWrite == true
                            select property;

                match.Each(x => x.SetValue(role, true));
            }

            return roles;
        }

        /// <summary>
        /// Nulls the FK objects in order to persist the entity
        /// </summary>
        /// <param name="entity"></param>
        public static T NullFKObjectsForPersistence<T>(this T entity)
            where T : class
        {
            foreach (var property in entity.GetType().GetRuntimeProperties())
            {
                try
                {
                    if (property.CanRead == false) continue;
                    if (property.CanWrite == false) continue;
                    if (property.PropertyType.IsPrimitive) continue;

                    //FK
                    if (property.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    {
                        property.SetValue(entity, null);
                        continue;
                    }

                    //Nullify it
                    if (property.GetCustomAttribute<NullWhenPersisting>() != null)
                    {
                        property.SetValue(entity, null);
                        continue;
                    }
                }
                catch
                {

                }
            }

            return entity;
        }

        /// <summary>
        /// Privacy policy (for the current language)
        /// </summary>
        /// <returns></returns>
        public static OrigamiPage? PrivacyPolicy(this IEnumerable<OrigamiPage> pages)
        {
            return pages
                .Published()
                .Where(x => x.Get().LanguageWrittenOn.StartsWith(_getLanguage()) == true)
                .Where(x => x.Keywords != null)
                .Where(x => x.Keywords!.Contains("privacy-policy"))
                .FirstOrDefault();
        }

        /// <summary>
        /// Be careful when calling this method, because it will cast all <typeparamref name="T"/> to <see cref="IFKBlog"/>
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        public static IEnumerable<T> Published<T>(this IEnumerable<T> entities, Guid blog)
            where T : IPublished, IDraft, IBlogIdNull
        {
            return entities.Blog(blog).Published();
        }

        public static string Published(this (IPublished?, Text) parameters)
        {
            if (parameters.Item1 == null) return string.Empty;
            return parameters.Item1.IsPublished ? $"{parameters.Item2.Lower("Yes")}, {parameters.Item1.DatePublished}" : parameters.Item2.Lower("No");
        }

        /// <summary>
        /// Be careful when calling this method, because it will cast all <typeparamref name="T"/> to <see cref="IPublished"/> and <see cref="IDeleted"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> Published<T>(this IEnumerable<T> entities)
            where T : IPublished
        {
            return from a in entities.NonDeleted()
                   where a.IsPublished
                   where a.DatePublished <= DateTime.UtcNow
                   select a;
        }

        /// <summary>
        /// Returns all entities given a <paramref name="filter"/> and <paramref name="order"/> from <paramref name="entities"/>
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static (int NumberOfRows, IEnumerable<T> Rows) Query<T>(this IEnumerable<T> entities, int take = 25, int skip = 0, string filter = "", string order = "")
        {
            var query = entities.AsQueryable();

            //filters the query
            if (filter.Has() == true) query = query.Where(filter);

            //orders the query
            if (order.Has() == true)
            {
                order = string.Join(",", order.Split(",", StringSplitOptions.RemoveEmptyEntries));
                query = query.OrderBy(order);
            }

            if (skip > 0) query = query.Skip(skip);
            if (take != int.MaxValue) if (take > 0) query = query.Take(take);

            //row count
            var rowNumber = entities.AsQueryable();
            if (filter.Has() == true) rowNumber = rowNumber.Where(filter);

            //returns the result
            return (rowNumber.Count(), query.ToList());
        }

        public static List<T> Read<T>(this DbContext db) where T : class
        {
            if (typeof(T).IsAbstract == false)
            {
                var t = Activator.CreateInstance<T>();
                return t switch
                {
                    OrigamiRole => [.. db.GetRolesFromDatabase().Cast<T>()],
                    _ => [.. db.Set<T>().AsNoTracking()],
                };
            }

            return [.. db.Set<T>().AsNoTracking()];
        }
        /// <summary>
        /// Tries to retrieve a blog by its slug. Returns null if not found or if the blog is deleted or inactive.
        /// </summary>
        /// <param name="blogRepository"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static OrigamiBlog? Slug(this IBlogRepository blogRepository, string slug)
        {
            var blogs = from b in blogRepository.ReadFromCache()
                        where b.IsDeleted == false
                        where b.IsActive == true
                        where b.Slug == slug
                        select b;

            return blogs.FirstOrDefault();
        }

        /// <summary>
        /// Terms and conditions (for the current language)
        /// </summary>
        /// <returns></returns>
        public static OrigamiPage? TermsAndConditions(this IEnumerable<OrigamiPage> pages)
        {
            return pages
                .Published()
                .Where(x => x.Get().LanguageWrittenOn.StartsWith(_getLanguage()) == true)
                .Where(x => x.Keywords != null)
                .Where(x => x.Keywords!.Contains("terms-and-conditions"))
                .FirstOrDefault();
        }

        /// <summary>
        /// Terms and conditions (for the current language)
        /// </summary>
        /// <returns></returns>
        public static OrigamiPage? TermsOfService(this IEnumerable<OrigamiPage> pages)
        {
            return pages
                .Published()
                .Where(x => x.Get().LanguageWrittenOn.StartsWith(_getLanguage()) == true)
                .Where(x => x.Keywords != null)
                .Where(x => x.Keywords!.Contains("terms-of-service"))
                .FirstOrDefault();
        }

        public static long Views<T>(this IRepository<T> read, IId entity, long views = -1)
            where T : class, IId
        {
            var key = entity.KeyForCachingViews();

            if (views >= 0) read.MemoryCache.Set(key, views);

            return read.MemoryCache.Get<long>(key);
        }

        /// <summary>
        /// Language for pages
        /// </summary>
        /// <returns></returns>
        private static string _getLanguage()
        {
            return CultureInfo.CurrentUICulture.Name.Split('-')[0];
        }
    }
}
