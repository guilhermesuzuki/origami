using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class RightRepository :
        RepositoryOuterLayer<OrigamiRight>,
        IRightRepository
    {
        protected readonly IRightRoleRepository _rightRoleRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public RightRepository(
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IRightRoleRepository rightRoleRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _rightRoleRepository = rightRoleRepository;
        }

        public Result KeepUpToDate()
        {
            using var db = DbContextFactory.CreateDbContext();

            var hub = new Result();
            var dbRoles = db.Set<OrigamiRight>().AsNoTracking().ToList();
            var uiRoles = OrigamiRole.GetRights();
            var merge = dbRoles.GetMergeRights(uiRoles);

            if (merge.Create.Any() == false)
            {
                hub.Simple = Text.Original("Roles are up-to-date");
                return hub;
            }

            merge.Create.GetContexts(new(OrigamiUser.AnonymousUser, DateTime.UtcNow)).Call(SmartCreate, false).Push(hub);
            hub.Simple = Text.Original("Roles have been created");

            return hub;
        }

        public override void PurgeRelationshipsFromCache(OrigamiRight entity)
        {
            base.PurgeRelationshipsFromCache(entity);
            var rightRoles = _rightRoleRepository.ReadFromCache().Where(x => x.RoleId == entity.Id).ToList();
            rightRoles.Each(_rightRoleRepository.PurgeCache);
        }

        public override Result<OrigamiRight> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiRight> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();
            var hub = base.PurgeRelationshipsFromDatabase(ctx);
            var rightRoles = db.Set<OrigamiRightRole>().AsNoTracking().Where(x => x.RoleId == ctx.Entity.Id).ToList();
            rightRoles.GetContexts(ctx).Call(_rightRoleRepository.SmartPurge, false).Push(hub);
            return hub;
        }
    }
}
