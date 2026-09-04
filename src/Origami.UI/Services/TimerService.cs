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
        private int _elapsedRunning;

        public TimerService(ISuperRepository superRepository, double timerInterval = 1000 * 60 * 3)
        {
            _super = superRepository;
            _timer = new() { AutoReset = true, Enabled = false, Interval = timerInterval };
            _timer.Elapsed += OnElapsed;
        }

        private void OnElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (System.Threading.Interlocked.Exchange(ref _elapsedRunning, 1) == 1) return;
            try
            {
                TimeToDoSomething(sender, e);
            }
            catch
            {
                // Best-effort background work: do not crash the host due to timer callback failures.
                // Consider injecting ILogger<TimerService> if you want to log this.
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref _elapsedRunning, 0);
            }
        }

        public override void Dispose()
        {
            _timer.Elapsed -= OnElapsed;
            _timer.Stop();
            _timer.Dispose();
            base.Dispose();
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
