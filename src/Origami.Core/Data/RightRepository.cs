using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IRightRoleRepository rightRoleRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _rightRoleRepository = rightRoleRepository;
        }

        public Result KeepUpToDate()
        {
            var hub = new Result();
            var dbRoles = this.ReadFromDatabase().ToList();
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
            var hub = base.PurgeRelationshipsFromDatabase(ctx);
            var rightRoles = _rightRoleRepository.ReadFromDatabase().Where(x => x.RoleId == ctx.Entity.Id).ToList();
            rightRoles.GetContexts(ctx).Call(_rightRoleRepository.SmartPurge, false).Push(hub);
            return hub;
        }
    }
}
