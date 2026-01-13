using Origami.Core.Data;
using System.Timers;

namespace Origami.UI.Services
{
    public class CacheRefreshServiceFull : CacheRefreshService
    {
        public CacheRefreshServiceFull(ISuperRepository superRepository) : base(superRepository)
        {

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            base.StartAsync(cancellationToken);
            _super.RefreshAllSearchIndexes();
            return Task.CompletedTask;
        }

        protected override void TimeToDoSomething(object? sender, ElapsedEventArgs e)
        {
            base.TimeToDoSomething(sender, e);
            _super.RefreshAllSearchIndexes();
        }
    }
}
