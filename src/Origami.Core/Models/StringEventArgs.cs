using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public class StringEventArgs : EventArgs
    {
        public string Value { get; }

        public StringEventArgs(string value)
        {
            Value = value;
        }
    }
}
