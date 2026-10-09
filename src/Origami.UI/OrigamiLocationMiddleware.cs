using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI
{
    internal class OrigamiLocationMiddleware(IMyMemoryCache _memoryCache, IIpLocationRepository _locationRepository, ILogger<OrigamiLocationMiddleware> _logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var key = $"Origami_UserLocation_{context.Connection.Id}";

                _logger.LogInformation("Checking for user location in cache with key: {Key}", key);

                if (_memoryCache.Get(key) is Location location)
                {
                    _logger.LogInformation("User location found in cache for key: {Key}", key);
                    await next(context).ConfigureAwait(false);
                    return;
                }

                var ip = context.Connection.RemoteIpAddress?.ToString();
                if (ip == null || ip.Like("::1") || ip.Like("127.0.0.1"))
                {
                    //needs to get the public ip address from 'localhost'
                    var url = "https://api.ipify.org/?format=json";
                    using var client = new HttpClient()
                    {
                        Timeout = TimeSpan.FromMilliseconds(250),
                    };
                    using var response = await client.GetAsync(url).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        using var json = System.Text.Json.JsonDocument.Parse(content);
                        if (json.RootElement.TryGetProperty("ip", out var ipElement))
                        {
                            ip = ipElement.GetString();
                        }
                    }
                }
                var result = await _locationRepository.GetLocationByIpAsync(ip!).ConfigureAwait(false);
                if (result.Ok)
                {
                    _logger.LogInformation("User location retrieved for key: {Key}", key);
                    _memoryCache.Set(key, result.Entity, TimeSpan.FromMinutes(20));
                }
            }
            catch (Exception)
            {

            }
            await next(context).ConfigureAwait(false);
        }
    }
}
