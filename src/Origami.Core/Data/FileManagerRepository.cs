using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class FileManagerRepository :
        RepositoryOuterLayer<OrigamiFile>,
        IFileManagerRepository
    {
        public FileManagerRepository(
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ManageFiles);
    }
}
