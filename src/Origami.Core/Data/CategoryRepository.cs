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
        protected readonly IContentCategoryRepository _contentCategoryRepository;
        protected readonly IValidator<OrigamiCategory> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public CategoryRepository(
            IValidator<OrigamiCategory> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IContentCategoryRepository contentCategoryRepository,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _contentCategoryRepository = contentCategoryRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewCategories);
        public override string DeletePermission => nameof(OrigamiRole.DeleteCategories);
        public override string PurgePermission => nameof(OrigamiRole.PurgeCategories);
        public override string ReadPermission => nameof(OrigamiRole.ViewCategories);
        public override string RestorePermission => nameof(OrigamiRole.RestoreCategories);
        public override string UpdatePermission => nameof(OrigamiRole.EditCategories);

        public override Result<OrigamiCategory> CreateValidation(DataOperationContext<OrigamiCategory> ctx)
        {
            return new Result<OrigamiCategory>(ctx.Entity, _validator);
        }

        public override void PurgeRelationshipsFromCache(OrigamiCategory entity)
        {
            var contentCategories = _contentCategoryRepository.ReadFromCache().Where(x => x.CategoryId == entity.Id);
            contentCategories.Each(_contentCategoryRepository.PurgeCache);
        }

        public override Result<OrigamiCategory> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiCategory> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();
            var hub = new Result<OrigamiCategory>(ctx.Entity);
            var row1 = db.Set<OrigamiContentCategory>().Where(x => x.CategoryId == ctx.Entity.Id).ExecuteDelete();
            hub.RowsAffected += row1;
            return hub;
        }

        public override Result<OrigamiCategory> UpdateValidation(DataOperationContext<OrigamiCategory> ctx)
        {
            return new Result<OrigamiCategory>(ctx.Entity, _validator);
        }
    }
}
