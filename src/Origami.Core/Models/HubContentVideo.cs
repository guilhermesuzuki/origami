using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public class HubContentVideo : HubContent<OrigamiVideo>
    {
        public HubContentVideo() : base()
        {
            this.Entity = new();
        }
    }
}
