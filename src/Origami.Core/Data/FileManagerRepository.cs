using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class FileManagerRepository :
        RepositoryOuterLayer<OrigamiFile>,
        IFileManagerRepository
    {
        public FileManagerRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {

        }

        public override string ReadPermission => nameof(OrigamiRole.ManageFiles);
    }
}
