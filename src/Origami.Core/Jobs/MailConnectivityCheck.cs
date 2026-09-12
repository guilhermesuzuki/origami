using Microsoft.Extensions.Logging;
using Origami.Core.Data;
using Origami.Core.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Jobs
{
    [DisallowConcurrentExecution]
    public class MailConnectivityCheck(
        IAppFacade AppFacade, 
        IEmailStatusRepository EmailStatusRepository, 
        ISuperRepository Super,
        ILogger<MailConnectivityCheck> Logger) : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Executing MailConnectivityCheck job.");

            var settings = Super.Settings.GetSettings();

            if (settings.KeepTestingTheSmtpServerConnectivity == false)
            {
                EmailStatusRepository.Status = null;
                AppFacade.RefreshUI(OrigamiConstants.Events.EmailStatus);
                return ValueTask.CompletedTask;
            }

            EmailStatusRepository.Status = Super.Emails.ConnectWithTheseSettings(settings);
            AppFacade.RefreshUI(OrigamiConstants.Events.EmailStatus);

            return ValueTask.CompletedTask;
        }
    }
}
