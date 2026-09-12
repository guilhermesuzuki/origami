using Microsoft.Extensions.Logging;
using Origami.Core.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Jobs
{
    public class CacheRefreshFull(ISuperRepository Super, ILogger<CacheRefreshFull> Logger) : IJob
    {
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Executing CacheRefreshFull job.");
            Super.RefreshAllRepositories();
            Super.RefreshAllSearchIndexes();
            return ValueTask.CompletedTask;
        }
    }
}
