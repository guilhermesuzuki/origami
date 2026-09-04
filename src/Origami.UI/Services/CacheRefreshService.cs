using Microsoft.Extensions.Hosting;
using Origami.Core.Data;

namespace Origami.UI.Services
{
    public class CacheRefreshService : BackgroundService
    {
        protected readonly ISuperRepository _super;
        private readonly System.Timers.Timer _timer;

        public CacheRefreshService(ISuperRepository superRepository) : base()
        {
            _super = superRepository;
            _timer = new() { AutoReset = true, Enabled = false, Interval = 1000 * 60 * 3 };
            _timer.Elapsed += TimeToDoSomething;
        }

        public override void Dispose()
        {
            base.Dispose();
            _timer.Dispose();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _super.RefreshAllRepositories();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer.Start();
            return Task.CompletedTask;
        }

        protected virtual void TimeToDoSomething(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _super.RefreshAllRepositories();
        }
    }
}
