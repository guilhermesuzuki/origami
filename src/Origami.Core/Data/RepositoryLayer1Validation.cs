using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer1Validation<T> :
        RepositoryLayer0Data<T>,
        ICreateValidation<T>,
        IUpdateValidation<T>,
        IDeleteValidation<T>,
        IPurgeValidation<T>
        where T : class, IId
    {
        protected RepositoryLayer1Validation(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual Result<T> CreateValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual Result<T> DeleteValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual Result<T> PurgeValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }

        public virtual Result<T> UpdateValidation(DataOperationContext<T> ctx)
        {
            return new(ctx.Entity);
        }
    }
}
