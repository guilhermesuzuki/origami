using Origami.Core.Models;

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
