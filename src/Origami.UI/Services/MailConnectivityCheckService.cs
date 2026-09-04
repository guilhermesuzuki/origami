using Microsoft.Extensions.Hosting;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Timers;

namespace Origami.UI.Services
{
    public class MailConnectivityCheckService : TimerService
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IEmailStatusRepository _emailStatus;

        public MailConnectivityCheckService(
            Text text,
            IAppFacade appFacade,
            IEmailStatusRepository emailStatusRepository,
            ISuperRepository superRepository
            ) : base(superRepository)
        {
            _appFacade = appFacade;
            _emailStatus = emailStatusRepository;
        }

        protected override void TimeToDoSomething(object? sender, ElapsedEventArgs e)
        {
            var settings = _super.Settings.GetSettings();

            if (settings.KeepTestingTheSmtpServerConnectivity == false)
            {
                _emailStatus.Status = null;
                _appFacade.RefreshUI(OrigamiConstants.Events.EmailStatus);
                return;
            }

            _emailStatus.Status = _super.Emails.ConnectWithTheseSettings(settings);
            _appFacade.RefreshUI(OrigamiConstants.Events.EmailStatus);
        }
    }
}
