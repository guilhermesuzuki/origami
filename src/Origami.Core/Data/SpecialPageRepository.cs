using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class SpecialPageRepository :
        RepositoryOuterLayer<OrigamiSpecialPage>,
        ISpecialPageRepository
    {
        protected readonly ISettingRepository _settingRepository;
        protected readonly IValidator<OrigamiSpecialPage> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public SpecialPageRepository(
            IAppFacade appFacade,
            IValidator<OrigamiSpecialPage> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            ISettingRepository settingRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _validator = validator;
            _settingRepository = settingRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewSpecialPages);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersSpecialPages);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnSpecialPages);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersSpecialPages);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnSpecialPages);
        public override string PurgePermission => nameof(OrigamiRole.PurgeSpecialPages);
        public override string ReadPermission => nameof(OrigamiRole.ViewSpecialPages);
        public override string RestorePermission => nameof(OrigamiRole.RestoreSpecialPages);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersSpecialPages);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnSpecialPages);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersSpecialPages);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnSpecialPages);

        public Result EnterMaintenanceMode(DataOperationContext context)
        {
            var permission = this.CheckPermission(context.User.Id, nameof(OrigamiRole.EnterMaintenanceMode));
            if (permission.Ok == false) return permission;

            using var db = this.DbContextFactory.CreateDbContext();

            var hub = new Result();
            var maintenancePages = db.Set<OrigamiSpecialPage>().AsNoTracking()
                .Where(x => x.IsPublished == false)
                .Where(x => x.Type == OrigamiSpecialPageTypes.Maintenance.ToString())
                .ToList();

            foreach (var page in maintenancePages)
            {
                if (hub.Ok == false) break;
                var ctx = new DataOperationContext<OrigamiSpecialPage>(context.User, page);
                this.SmartPublish(ctx, true).Push(hub);
            }
            this._settingRepository.UpdateOnlyThisSetting(context, nameof(OrigamiSettings.MaintenanceMode).ToLower(), true.ToString()).Push(hub);
            return hub;
        }

        public Result LeaveMaintenanceMode(DataOperationContext context)
        {
            var permission = this.CheckPermission(context.User.Id, nameof(OrigamiRole.LeaveMaintenanceMode));
            if (permission.Ok == false) return permission;

            using var db = this.DbContextFactory.CreateDbContext();

            var hub = new Result();
            var maintenancePages = db.Set<OrigamiSpecialPage>().AsNoTracking()
                .Where(x => x.IsPublished)
                .Where(x => x.Type == OrigamiSpecialPageTypes.Maintenance.ToString())
                .ToList();

            foreach (var page in maintenancePages)
            {
                if (hub.Ok == false) break;
                var ctx = new DataOperationContext<OrigamiSpecialPage>(context.User, page);
                this.SmartUnpublish(ctx, true).Push(hub);
            }
            this._settingRepository.UpdateOnlyThisSetting(context, nameof(OrigamiSettings.MaintenanceMode).ToLower(), false.ToString()).Push(hub);
            return hub;
        }

        public override Result<OrigamiSpecialPage> CreateValidation(DataOperationContext<OrigamiSpecialPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiSpecialPage> DeleteValidation(DataOperationContext<OrigamiSpecialPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiSpecialPage> PurgeValidation(DataOperationContext<OrigamiSpecialPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        public override Result<OrigamiSpecialPage> UpdateValidation(DataOperationContext<OrigamiSpecialPage> ctx)
        {
            return _validationForAllOperations(ctx);
        }

        private Result<OrigamiSpecialPage> _validationForAllOperations(DataOperationContext<OrigamiSpecialPage> ctx)
        {
            Result<OrigamiSpecialPage> result = new(ctx.Entity);
            result.Error = Text.Original("Operation not allowed");
            result.Error = Text.Original("Use the HubContentSpecialPage repository instead");
            return result;
        }
    }
}
