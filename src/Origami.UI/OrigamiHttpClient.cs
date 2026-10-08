using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.UI
{
    public class OrigamiHttpClient(IHttpContextAccessor accessor, HttpClient httpClient)
    {
        public HttpClient GetHttpClient()
        {
            var uri = new UriBuilder(
                accessor.HttpContext!.Request.Scheme,
                accessor.HttpContext!.Request.Host.Host,
                accessor.HttpContext!.Request.Host.Port ?? 80
                ).Uri;

            httpClient.BaseAddress = uri;
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            return httpClient;
        }
    }
}
