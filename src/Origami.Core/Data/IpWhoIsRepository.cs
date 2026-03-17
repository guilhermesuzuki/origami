using Origami.Core.Models;
using System.Text.Json;

namespace Origami.Core.Data
{
    /// <summary>
    /// https://ipwho.is/
    /// </summary>
    public class IpWhoIsRepository : IIpLocationRepository
    {
        public const string Host = "ipwho.is";
        protected Text Text;

        public IpWhoIsRepository(Text text) : base()
        {
            Text = text;
        }

        public async Task<Result<Location>> GetLocationByIpAsync(string ip)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri($"https://{Host}/"),
                Timeout = TimeSpan.FromMilliseconds(500),
                DefaultRequestVersion = new Version(2, 0),
            };

            try
            {
                var response = await client.GetAsync(ip).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(content);
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("success", out var success) && success.GetBoolean())
                    {
                        var location = new Location
                        {
                            City = root.GetProperty("city").GetString()!,
                            Country = root.GetProperty("country").GetString()!,
                            CountryCode = root.GetProperty("country_code").GetString()!,
                            Latitude = (float)root.GetProperty("latitude").GetDouble(),
                            Longitude = (float)root.GetProperty("longitude").GetDouble(),
                            Region = root.GetProperty("region").GetString()!,
                            RegionCode = root.GetProperty("region_code").GetString()!,
                            ZipCode = root.GetProperty("postal").GetString()!,
                            Provider = Host,
                        };

                        var timeZone = root.GetProperty("timezone");
                        var utc = timeZone.GetProperty("utc").GetString()!;

                        var f = utc[0] switch
                        {
                            '+' => +1,
                            '-' => -1,
                            _ => +1
                        };

                        var hours = int.Parse(utc.Substring(1, 2));
                        var minutes = int.Parse(utc.Substring(4, 2));

                        location.TimeZone = timeZone.GetProperty("id").GetString()!;
                        location.TimeZoneOffset = f * (hours * 60 + minutes);

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
