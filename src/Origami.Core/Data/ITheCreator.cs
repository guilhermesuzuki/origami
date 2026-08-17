using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface ITheCreator
    {
        T Create<T>() where T : class, new();
    }
}
