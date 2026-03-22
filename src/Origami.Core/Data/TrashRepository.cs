using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class TrashRepository :
        RepositoryOuterLayer<OrigamiTrash>,
        ITrashRepository
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ISpecialPageRepository _specialPageRepository;
        private readonly ISpecialMessageRepository _specialMessageRepository;

        public TrashRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IBlogRepository blogRepository,
            ISpecialPageRepository specialPageRepository,
            ISpecialMessageRepository specialMessageRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _blogRepository = blogRepository;
            _specialPageRepository = specialPageRepository;
            _specialMessageRepository = specialMessageRepository;
        }

        public override string ReadPermission => nameof(OrigamiRole.ViewTrashes);

        public override List<OrigamiTrash> Search(string searchTerm)
        {
            using var db = DbContextFactory.CreateDbContext();

            var query = from x in db.Set<OrigamiTrash>().AsNoTracking()
                        where x.Type.Contains(searchTerm) ||
                              x.Name.Contains(searchTerm) ||
                              x.Title.Contains(searchTerm) ||
                              x.Content.Contains(searchTerm)
                        orderby x.Type, string.IsNullOrWhiteSpace(x.Title) == false ? x.Title : x.Name
                        select x;

            return query.ToList();
        }

        public override Result<OrigamiTrash> SmartPurge(DataOperationContext<OrigamiTrash> ctx, bool checkPermission)
        {
            if (ctx.Entity.Type.Like("Blog") == true)
            {
                return _purge(_blogRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialPage") == true)
            {
                return _purge(_specialPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialMessage") == true)
            {
                return _purge(_specialMessageRepository, ctx);
            }

            throw new NotImplementedException();
        }

        public override Result<OrigamiTrash> SmartRestore(DataOperationContext<OrigamiTrash> ctx, bool checkPermission)
        {
            if (ctx.Entity.Type.Like("Blog") == true)
            {
                return _restore(_blogRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialPage") == true)
            {
                return _restore(_specialPageRepository, ctx);
            }

            if (ctx.Entity.Type.Like("SpecialMessage") == true)
            {
                return _restore(_specialMessageRepository, ctx);
            }

            throw new NotImplementedException();
        }

        private Result<OrigamiTrash> _purge<T>(IRepository<T> repo, DataOperationContext<OrigamiTrash> trash)
            where T : class, IId, new()
        {
            var hub = new Result<OrigamiTrash>(trash.Entity);
            var entity = repo.ReadFromDatabase(trash.Entity);
            var ctx = new DataOperationContext<T>(trash.User, trash.DateTime, entity ?? new());
            return repo.SmartPurge(ctx, true).Push(hub);
        }

        private Result<OrigamiTrash> _restore<T>(IRepository<T> repo, DataOperationContext<OrigamiTrash> trash)
            where T : class, IId, new()
        {
            var hub = new Result<OrigamiTrash>(trash.Entity);
            var entity = repo.ReadFromDatabase(trash.Entity);
            var ctx = new DataOperationContext<T>(trash.User, trash.DateTime, entity ?? new());
            if (entity != null)
            {
                return repo.SmartRestore(ctx, true).Push(hub);
            }
            return new(trash.Entity) { Error = Text.Original("Unable to restore trash") };
        }
    }
}
