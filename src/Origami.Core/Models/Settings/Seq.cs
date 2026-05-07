using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models.Settings
{
    public class Seq : IEnabled, IEndpoint
    {
        public Seq() { }
        public bool Enabled { get; set; }
        public string Endpoint { get; set; } = string.Empty;
    }
}
