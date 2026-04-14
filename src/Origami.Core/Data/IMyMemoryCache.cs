using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface IMyMemoryCache : IMemoryCache
    {
        List<T> Read<T>() where T : class;
    }
}
