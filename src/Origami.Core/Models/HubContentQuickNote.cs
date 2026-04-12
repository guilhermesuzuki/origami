using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public class HubContentQuickNote : HubContent<OrigamiQuickNote>
    {
        public HubContentQuickNote() : base()
        {
            this.Entity = new();
        }
    }
}
