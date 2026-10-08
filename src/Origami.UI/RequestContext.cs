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
        public string? ConnectionId { get; set; }
        public IDictionary<string, string>? Headers { get; set; }
        public string? Host { get; set; }
        public string? IpAddress { get; set; }
        public Location? Location { get; set; }
        public string? Referrer { get; set; }
        public string? Scheme { get; set; }
        public string? UserAgent { get; set; }
    }
}
