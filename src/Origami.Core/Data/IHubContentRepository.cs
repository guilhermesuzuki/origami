using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface IHubContentRepository<T>
    {
        // need to get by id async
        T Get(IId entityId);

        // need to save
        Result<T> Save(T entity, IId userId);
    }
}
