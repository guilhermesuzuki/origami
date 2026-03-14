using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class RoleRepository :
        RepositoryOuterLayer<OrigamiRole>,
        IRoleRepository
    {
        protected readonly IRightRepository _rightRepository;
        protected readonly IRightRoleRepository _rightRoleRepository;
        protected readonly IUserRoleRepository _userRoleRepository;
        protected readonly IValidator<OrigamiRole> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public RoleRepository(
            IValidator<OrigamiRole> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IRightRepository rightRepository,
            IRightRoleRepository rightRoleRepository,
            IUserRoleRepository userRoleRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _rightRepository = rightRepository;
            _rightRoleRepository = rightRoleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewRoles);
        public override string DeletePermission => nameof(OrigamiRole.DeleteRoles);
        public override string PurgePermission => nameof(OrigamiRole.PurgeRoles);
        public override string ReadPermission => nameof(OrigamiRole.ViewRoles);
        public override string RestorePermission => nameof(OrigamiRole.RestoreRoles);
        public override string UpdatePermission => nameof(OrigamiRole.EditRoles);

        public override Result<OrigamiRole> CanUpdate(DataOperationContext<OrigamiRole> ctx)
        {
            var hub = base.CanUpdate(ctx);

            var fresh = ReadFromDatabase().Id(ctx.Entity.Id)!;
            if (fresh.IsSystemRole)
            {
                var permission = CheckPermission(ctx, nameof(OrigamiRole.EditSystemRoles));
                if (permission.Ok == false)
                {
                    hub.ErrorMessage = Text.Original("You cannot edit system roles");
                }
            }

            return hub;
        }

        public override Result<OrigamiRole> Create(DataOperationContext<OrigamiRole> ctx)
        {
            var hub = base.Create(ctx);
            var rights = _rightRepository.ReadFromDatabase().ToList();
            return ctx.Entity.GetRightRoles(rights).GetContexts(ctx).Call(_rightRoleRepository.SmartSave, false).Push(hub);
        }

        public override void CreateCache(OrigamiRole entity)
        {
            var rights = _rightRepository.ReadFromDatabase().ToList();
            entity.GetRightRoles(rights).Each(_rightRoleRepository.CreateCache);
            base.CreateCache(entity);
        }

        public override Result<OrigamiRole> CreateValidation(DataOperationContext<OrigamiRole> ctx)
        {
            return new Result<OrigamiRole>(ctx.Entity, _validator);
        }

        public override Result<OrigamiRole> DeleteValidation(DataOperationContext<OrigamiRole> ctx)
        {
            return new Result<OrigamiRole>(ctx.Entity, _validator);
        }

        public override void PurgeRelationshipsFromCache(OrigamiRole entity)
        {
            base.PurgeRelationshipsFromCache(entity);

            _rightRoleRepository.ReadFromCache()
                .Where(x => x.RoleId == entity.Id)
                .Each(_rightRoleRepository.PurgeCache);

            _userRoleRepository.ReadFromCache()
                .Where(x => x.RoleId == entity.Id)
                .Each(_userRoleRepository.PurgeCache);
        }

        public override Result<OrigamiRole> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiRole> ctx)
        {
            var hub = new Result<OrigamiRole>(ctx.Entity);
            using (var db = DbContextFactory.CreateDbContext())
            {
                var roles1 = from x in db.RightRoles
                             where x.RoleId == ctx.Entity.Id
                             select x;

                var roles2 = from x in db.UserRoles
                             where x.RoleId == ctx.Entity.Id
                             select x;

                hub.RowsAffected += roles1.ExecuteDelete();
                hub.RowsAffected += roles2.ExecuteDelete();
            }
            return hub;
        }

        public override IQueryable<OrigamiRole> ReadFromDatabase()
        {
            using (var ctx = this.DbContextFactory.CreateDbContext())
            {
                var roles = ctx.Roles.ToList();

                foreach (var role in roles)
                {
                    var rightRoles = _rightRoleRepository.ReadFromDatabase().Where(x => x.RoleId == role.Id).ToList();

                    var match = from property in role.GetType().GetProperties()
                                join rt in ctx.Rights on property.Name equals rt.Name
                                join rr in rightRoles on rt.Id equals rr.RightId
                                where property.CanWrite == true
                                select property;

                    match.Each(x => x.SetValue(role, true));
                }

                return roles.AsQueryable();
            }
        }

        public override Result<OrigamiRole> Update(DataOperationContext<OrigamiRole> ctx)
        {
            var result = base.Update(ctx);
            var rights = _rightRepository.ReadFromDatabase().ToList();
            var ui = ctx.Entity.GetRightRoles(rights);
            var db = _rightRoleRepository.ReadFromDatabase().Where(x => x.RoleId == ctx.Entity.Id).ToList();
            var merge = db.GetMergeRightRoles(ui);
            merge.Purge.GetContexts(ctx).Call(_rightRoleRepository.SmartPurge, false).Push(result);
            merge.Create.GetContexts(ctx).Call(_rightRoleRepository.SmartSave, false).Push(result);
            return result;
        }

        public override void UpdateCache(OrigamiRole entity)
        {
            base.UpdateCache(entity);

            var rights = _rightRepository.ReadFromDatabase().ToList();
            var uiRights = entity.GetRightRoles(rights);
            var cache = _rightRoleRepository.ReadFromCache().Where(x => x.RoleId == entity.Id).ToList();
            var merge = cache.GetMergeRightRoles(uiRights);

            merge.Purge.Each(_rightRoleRepository.PurgeCache);
            merge.Create.Each(_rightRoleRepository.CreateCache);
        }
        public override Result<OrigamiRole> UpdateValidation(DataOperationContext<OrigamiRole> ctx)
        {
            return new Result<OrigamiRole>(ctx.Entity, _validator);
        }
    }
}
