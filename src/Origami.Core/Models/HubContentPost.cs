using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public class HubContentPost : HubContent<OrigamiPost>
    {
        public HubContentPost() : base()
        {
            this.Entity = new();
        }
    }
}
