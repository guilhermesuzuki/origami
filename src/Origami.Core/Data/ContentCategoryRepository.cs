using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class ContentCategoryRepository :
        RepositoryOuterLayer<OrigamiContentCategory>,
        IContentCategoryRepository
    {
        protected readonly IValidator<OrigamiContentCategory> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentCategoryRepository(
            IAppFacade appFacade,
            IValidator<OrigamiContentCategory> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _validator = validator;
        }
    }
}
