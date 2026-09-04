using Microsoft.Extensions.Hosting;
using Origami.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.UI.Services
{
    public abstract class TimerService : BackgroundService
    {
        protected readonly ISuperRepository _super;
        private readonly System.Timers.Timer _timer;

        public TimerService(ISuperRepository superRepository)
        {
            _super = superRepository;
            _timer = new() { AutoReset = true, Enabled = false, Interval = 1000 * 60 * 3 };
            _timer.Elapsed += TimeToDoSomething;
        }

        public override void Dispose()
        {
            base.Dispose();
            this._timer.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer.Start();

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // expected during shutdown
            }
            finally
            {
                _timer.Stop();
            }
        }

        protected abstract void TimeToDoSomething(object? sender, System.Timers.ElapsedEventArgs e);
    }
}
