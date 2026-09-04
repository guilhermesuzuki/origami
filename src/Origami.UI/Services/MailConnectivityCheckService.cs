using Microsoft.Extensions.Hosting;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI.Services
{
    public class MailConnectivityCheckService : BackgroundService
    {
        protected const int _timerInterval = 1000 * 60 * 3;
        protected readonly IAppFacade _appFacade;
        protected readonly IEmailStatusRepository _emailStatus;
        protected readonly ISuperRepository _super;
        protected readonly Text _text;
        private System.Timers.Timer _timer;

        public MailConnectivityCheckService(
            Text text,
            IAppFacade appFacade,
            IEmailStatusRepository emailStatusRepository,
            ISuperRepository superRepository
            ) : base()
        {
            _text = text;
            _appFacade = appFacade;
            _super = superRepository;
            _emailStatus = emailStatusRepository;
            _timer = new() { AutoReset = true, Enabled = false, Interval = _timerInterval };
            _timer.Elapsed += TimeToDoSomething;
        }

        public override void Dispose()
        {
            base.Dispose();
            _timer.Dispose();
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
            var settings = _super.Settings.GetSettings();
            _emailStatus.Status = settings.KeepTestingTheSmtpServerConnectivity ? _super.Emails.ConnectWithTheseSettings(settings) : null;
            _appFacade.RefreshUI(OrigamiConstants.Events.EmailStatus);
        }
    }
}
