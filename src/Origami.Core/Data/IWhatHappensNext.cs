using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    /// <summary>
    /// What happens next hub
    /// </summary>
    public interface IWhatHappensNext
    {
        event EventHandler<WhenIClickHereEventArgs> WhenClickingHere;

        void WhenIClickHere(object? sender, WhenIClickHereEventArgs e);
    }
}
