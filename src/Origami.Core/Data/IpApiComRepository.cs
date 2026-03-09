using Origami.Core.Models;
using System.Text.Json;

namespace Origami.Core.Data
{
    /// <summary>
    /// https://ipapi.co/
    /// </summary>
    public class IpApiComRepository : IIpLocationRepository
    {
        public const string Host = "ip-api.com";
        protected Text Text;

        public IpApiComRepository(Text text) : base()
        {
            this.Text = text;
        }

        public async Task<Result<Location>> GetLocationByIpAsync(string ip)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri($"http://{Host}/"),
                Timeout = TimeSpan.FromMilliseconds(500),
                DefaultRequestVersion = new Version(2, 0),
            };

            try
            {
                var response = await client.GetAsync($"/json/{ip}?fields=status,message,country,countryCode,region,regionName,city,zip,lat,lon,timezone,offset,isp,org,as,query").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(content);
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("status", out var success) && success.GetString().Like("success") == true)
                    {
                        var location = new Location
                        {
                            City = root.GetProperty("city").GetString()!,
                            Country = root.GetProperty("country").GetString()!,
                            CountryCode = root.GetProperty("countryCode").GetString()!,
                            Latitude = (float)root.GetProperty("lat").GetDouble(),
                            Longitude = (float)root.GetProperty("lon").GetDouble(),
                            Region = root.GetProperty("regionName").GetString()!,
                            RegionCode = root.GetProperty("region").GetString()!,
                            ZipCode = root.GetProperty("zip").GetString()!,
                            TimeZone = root.GetProperty("timezone").GetString()!,
                            Provider = Host,
                        };

                        var offset = root.GetProperty("offset").GetInt32();
                        location.UtcOffset = TimeSpan.FromSeconds(offset);

                        return new(location);
                    }
                }

                return new() { ErrorMessage = $"HTTP {response.StatusCode} - {response.ReasonPhrase} from {Host}", };
            }
            catch
            {

            }

            return new() { ErrorMessage = Text.Original("Unable to retrieve IP location") };
        }
    }
}
