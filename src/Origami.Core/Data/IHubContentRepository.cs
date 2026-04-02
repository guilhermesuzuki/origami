using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IHubContentRepository<T>
    {
        // need to get by id async
        T Get(IId entityId);

        Result CanRead(IId userId);
        Result<T> Save(T entity, IId userId);
        Result<T> Delete(T entity, IId userId);
        Result<T> Purge(T entity, IId userId);
        Result<T> Restore(T entity, IId userId);
        Result<T> Publish(T entity, IId userId);
        Result<T> Unpublish(T entity, IId userId);
        Result<T> PromoteToFrontPage(T entity, IId userId);
        Result<T> DemoteFromFrontPage(T entity, IId userId);
    }
}
