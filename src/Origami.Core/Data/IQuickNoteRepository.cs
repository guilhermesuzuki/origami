using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public interface IQuickNoteRepository : 
        IRepository<OrigamiQuickNote>, 
        IPublish<OrigamiQuickNote>
    {

    }
}
