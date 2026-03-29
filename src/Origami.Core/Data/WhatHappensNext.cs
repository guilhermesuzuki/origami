using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public class WhatHappensNext : IWhatHappensNext
    {
        public event EventHandler<WhenIClickHereEventArgs> WhenClickingHere = delegate(object? sender, WhenIClickHereEventArgs eventArgs)
        {

        };

        public void WhenIClickHere(object? sender, WhenIClickHereEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
