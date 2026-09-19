using Origami.Core.Data;
using System.Timers;

namespace Origami.UI.Services
{
    public class CacheRefreshServiceFull : TimerService
    {
        public CacheRefreshServiceFull(ISuperRepository superRepository) : base(superRepository)
        {
            
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _super.RefreshAllRepositories();
            _super.RefreshAllSearchIndexes();
            await base.StartAsync(cancellationToken);
        }

        protected override void TimeToDoSomething(object? sender, ElapsedEventArgs e)
        {
            _super.RefreshAllRepositories();
            _super.RefreshAllSearchIndexes();
        }
    }
}
