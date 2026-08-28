using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class ContentRepository :
        RepositoryOuterLayer<OrigamiContent>,
        IContentRepository
    {
        protected readonly IValidator<OrigamiContent> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentRepository(
            IAppFacade appFacade,
            IValidator<OrigamiContent> validator,
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
