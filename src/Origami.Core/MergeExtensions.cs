using Origami.Core.Models;

namespace Origami.Core
{
    public static class MergeExtensions
    {
        /// <summary>
        /// Merges <paramref name="dbRows"/> and <paramref name="uiRows"/> by <see cref="IId.Id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbRows">Rows that come straight from the source, the database</param>
        /// <param name="uiRows">Rows that come straight from the UI, that need to be persisted in the database</param>
        /// <returns></returns>
        public static (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) GetMerge<T>(this IEnumerable<T> dbRows, IEnumerable<T> uiRows)
            where T : IId
        {
            if (dbRows.Any() == false)
            {
                return ([], [], uiRows);
            }

            var join = from a in dbRows
                       join b in uiRows on a.Id equals b.Id
                       select new { db = a, ui = b };

            var update = join.Select(x => x.ui);
            var add = uiRows.Except(update);
            var purge = dbRows.Except(join.Select(x => x.db));

            return (purge, update, add);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbRows">Rows that come straight from the source, the database</param>
        /// <param name="uiRows">Rows that come straight from the UI, that need to be persisted in the database</param>
        /// <returns></returns>
        public static (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) GetMergeCategories<T>(this IEnumerable<T> dbRows, IEnumerable<T> uiRows)
            where T : IId, ICategoryId
        {
            if (dbRows.Any() == false)
            {
                return ([], [], uiRows);
            }

            var join = from a in dbRows
                       join b in uiRows on a.Id equals b.Id
                       select new { db = a, ui = b };

            var update = join.Select(x => x.ui);
            var create = uiRows.Except(update);
            var purge = dbRows.Except(join.Select(x => x.db));

            return (purge, update, create);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbRows"></param>
        /// <param name="uiRows"></param>
        /// <returns></returns>
        public static (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) GetMergeRightRoles<T>(this IEnumerable<T> dbRows, IEnumerable<T> uiRows)
            where T : OrigamiRightRole
        {
            if (dbRows.Any() == false)
            {
                return ([], [], uiRows);
            }

            var join = from a in dbRows
                       join b in uiRows on a.Id equals b.Id
                       select new { db = a, ui = b };

            //needs to extract the id, if this is the case
            join.Each(row =>
            {
                if (row.ui is IId uiId && row.db is IId dbId) uiId.Id = dbId.Id;
            });

            var update = join.Select(x => x.ui);
            var add = uiRows.Except(update);
            var purge = dbRows.Except(join.Select(x => x.db));

            return (purge, update, add);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbRows"></param>
        /// <param name="uiRows"></param>
        /// <returns></returns>
        public static (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) GetMergeRights<T>(this IEnumerable<T> dbRows, IEnumerable<T> uiRows)
            where T : OrigamiRight
        {
            if (dbRows.Any() == false)
            {
                return ([], [], uiRows);
            }

            var join = from a in dbRows
                       join b in uiRows on a.Name equals b.Name
                       select new { db = a, ui = b };

            //needs to extract the id, if this is the case
            join.Each(row =>
            {
                if (row.ui is IId uiId && row.db is IId dbId) uiId.Id = dbId.Id;
            });

            var update = join.Select(x => x.ui);
            var add = uiRows.Except(update);
            var purge = dbRows.Except(join.Select(x => x.db));

            return (purge, update, add);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbRows"></param>
        /// <param name="uiRows"></param>
        /// <returns></returns>
        public static (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) GetMergeSettings<T>(this IEnumerable<T> dbRows, IEnumerable<T> uiRows)
            where T : BaseSetting
        {
            if (dbRows.Any() == false)
            {
                return ([], [], uiRows);
            }

            var join = from a in dbRows
                       join b in uiRows on new { a.BlogId, a.Username, a.Name } equals new { b.BlogId, b.Username, b.Name }
                       select new { db = a, ui = b };

            //needs to extract the id, if this is the case
            join.Each(row =>
            {
                if (row.ui is IId uiId && row.db is IId dbId) uiId.Id = dbId.Id;
            });

            var update = join.Select(x => x.ui);
            var add = uiRows.Except(update);
            var purge = dbRows.Except(join.Select(x => x.db));

            return (purge, update, add);
        }

        /// <summary>
        /// TODO: comment this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbRows">Rows that come straight from the source, the database</param>
        /// <param name="uiRows">Rows that come straight from the UI, that need to be persisted in the database</param>
        /// <returns></returns>
        public static (IEnumerable<T> Purge, IEnumerable<T> Update, IEnumerable<T> Create) GetMergeTags<T>(this IEnumerable<T> dbRows, IEnumerable<T> uiRows)
            where T : ITag, IId
        {
            if (dbRows.Any() == false)
            {
                return ([], [], uiRows);
            }

            var join = from a in dbRows
                       join b in uiRows on a.Id equals b.Id
                       select new { db = a, ui = b };

            var update = join.Select(x => x.ui);
            var create = uiRows.Except(update);
            var purge = dbRows.Except(join.Select(x => x.db));

            return (purge, update, create);
        }
    }
}
