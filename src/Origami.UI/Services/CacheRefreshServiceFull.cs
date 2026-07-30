using Origami.Core.Data;
using System.Timers;

namespace Origami.UI.Services
{
    public class CacheRefreshServiceFull : CacheRefreshService
    {
        public CacheRefreshServiceFull(ISuperRepository superRepository) : base(superRepository)
        {

        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            _super.RefreshAllSearchIndexes();
        }

        protected override void TimeToDoSomething(object? sender, ElapsedEventArgs e)
        {
            base.TimeToDoSomething(sender, e);
            _super.RefreshAllSearchIndexes();
        }
    }
}
