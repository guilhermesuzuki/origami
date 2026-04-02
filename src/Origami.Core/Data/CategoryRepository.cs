using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class CategoryRepository :
        RepositoryOuterLayer<OrigamiCategory>,
        ICategoryRepository
    {
        protected readonly IValidator<OrigamiCategory> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public CategoryRepository(
            IValidator<OrigamiCategory> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewCategories);
        public override string DeletePermission => nameof(OrigamiRole.DeleteCategories);
        public override string PurgePermission => nameof(OrigamiRole.PurgeCategories);
        public override string ReadPermission => nameof(OrigamiRole.ViewCategories);
        public override string RestorePermission => nameof(OrigamiRole.RestoreCategories);
        public override string UpdatePermission => nameof(OrigamiRole.EditCategories);

        public override Result<OrigamiCategory> CreateValidation(DataOperationContext<OrigamiCategory> ctx)
        {
            var validation = new Result<OrigamiCategory>(ctx.Entity, _validator);

            if (this.IsCycleDetected(ctx, []) == true)
            {
                validation.Error = $"Cycle detected: you must choose another parent";
            }

            this.ValidateSlug(ctx).Push(validation);

            return validation;
        }

        public override void PurgeRelationshipsFromCache(OrigamiCategory entity)
        {
            //var row1 = _postCategoryRepository.ReadFromCache().Where(x => x.CategoryId == entity.Id);
            //var row2 = _videoCategoryRepository.ReadFromCache().Where(x => x.CategoryId == entity.Id);

            //row1.Each(_postCategoryRepository.PurgeCache);
            //row2.Each(_videoCategoryRepository.PurgeCache);
        }

        public override Result<OrigamiCategory> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiCategory> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();
            var hub = new Result<OrigamiCategory>(ctx.Entity);
            //var row1 = db.Set<OrigamiPostCategory>().Where(x => x.CategoryId == ctx.Entity.Id).ExecuteDelete();
            //var row2 = db.Set<OrigamiVideoCategory>().Where(x => x.CategoryId == ctx.Entity.Id).ExecuteDelete();
            //hub.RowsAffected += row1;
            //hub.RowsAffected += row2;
            return hub;
        }

        public override Result<OrigamiCategory> UpdateValidation(DataOperationContext<OrigamiCategory> ctx)
        {
            var validation = new Result<OrigamiCategory>(ctx.Entity, _validator);

            if (this.IsCycleDetected(ctx, []) == true)
            {
                validation.Error = $"Cycle detected: you must choose another parent";
            }

            this.ValidateSlug(ctx).Push(validation);

            return validation;
        }
    }
}
