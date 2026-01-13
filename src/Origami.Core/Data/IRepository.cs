using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IRepository<T> :
        IBaseRepository<T>,
        ICrud<T>,
        ICache<T>,
        ICreateValidation<T>,
        IUpdateValidation<T>,
        IDeleteValidation<T>,
        ICreatePermission<T>,
        IReadPermission<T>,
        IUpdatePermission<T>,
        IDeletePermission<T>,
        IPublishPermission<T>,
        ISmartDelete<T>,
        ISmartPurge<T>,
        ISmartRestore<T>,
        ISmartSave<T>,
        IMerge<T>,
        ISearch<T>
        where T : IId
    {

    }
}
