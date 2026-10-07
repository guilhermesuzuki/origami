using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Models
{
    public sealed class RequestContext
    {
        public RequestContext(ILogger<RequestContext> logger, IHttpContextAccessor accessor)
        {
            var httpContext = accessor.HttpContext;
            if (httpContext is null) return;

            ConnectionId = httpContext.Connection.Id;
            Headers = httpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            Host = httpContext.Request.Host.Value;
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            Referrer = httpContext.Request.Headers["Referer"].ToString();
            Scheme = httpContext.Request.Scheme;
            UserAgent = httpContext.Request.Headers["User-Agent"].ToString();

            logger.LogInformation("RequestContext connection ID: {ConnectionId}, Host: {Host}, IP Address: {IpAddress}, Referrer: {Referrer}, Scheme: {Scheme}, User-Agent: {UserAgent}", ConnectionId, Host, IpAddress, Referrer, Scheme, UserAgent);
        }

        public string? ConnectionId { get; }
        public IDictionary<string, string>? Headers { get; }
        public string? Host { get; }
        public string? IpAddress { get; }
        public string? Referrer { get; }
        public string? Scheme { get; }
        public string? UserAgent { get; }
    }
}
