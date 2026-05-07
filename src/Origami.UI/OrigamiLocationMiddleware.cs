using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI
{
    internal class OrigamiLocationMiddleware : IMiddleware
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IIpLocationRepository _locationRepository;

        public OrigamiLocationMiddleware(IMemoryCache memoryCache, IIpLocationRepository locationRepository)
        {
            _memoryCache = memoryCache;
            _locationRepository = locationRepository;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                Console.WriteLine($"Connection Id: {context.Connection.Id}");

                var key = $"Origami_UserLocation_{context.Connection.Id}";

                if (_memoryCache.Get(key) is Location location)
                {
                    await next(context);
                    return;
                }

                var ip = context.Connection.RemoteIpAddress?.ToString();
                if (ip == null || ip.Like("::1") || ip.Like("127.0.0.1"))
                {
                    //needs to get the public ip address from 'localhost'
                    var url = "https://api.ipify.org/?format=json";
                    using var client = new HttpClient()
                    {
                        Timeout = TimeSpan.FromMilliseconds(150),
                    };
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var json = System.Text.Json.JsonDocument.Parse(content);
                        if (json.RootElement.TryGetProperty("ip", out var ipElement))
                        {
                            ip = ipElement.GetString();
                        }
                    }
                }
                var result = await _locationRepository.GetLocationByIpAsync(ip!);
                if (result.Ok)
                {
                    _memoryCache.Set(key, result.Entity, TimeSpan.FromHours(1));
                }
            }
            catch (Exception)
            {

            }
            await next(context);
        }
    }
}
