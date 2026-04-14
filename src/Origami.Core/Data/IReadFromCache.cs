using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface IReadFromCache<T>
    {
        List<T> ReadFromCache();
    }
}
