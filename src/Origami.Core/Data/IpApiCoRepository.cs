using Origami.Core.Models;
using System.Text.Json;

namespace Origami.Core.Data
{
    /// <summary>
    /// https://ipapi.co/
    /// </summary>
    public class IpApiCoRepository : IIpLocationRepository
    {
        public const string Host = "ipapi.co";
        protected Text Text;

        public IpApiCoRepository(Text text) : base()
        {
            this.Text = text;
        }

        public async Task<Result<Location>> GetLocationByIpAsync(string ip)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri($"https://{Host}/"),
                Timeout = TimeSpan.FromMilliseconds(150),
                DefaultRequestVersion = new Version(2, 0),
            };

            try
            {
                var response = await client.GetAsync($"{ip}/json").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(content);
                    JsonElement root = doc.RootElement;

                    var location = new Location
                    {
                        City = root.GetProperty("city").GetString()!,
                        Country = root.GetProperty("country_name").GetString()!,
                        CountryCode = root.GetProperty("country_code").GetString()!,
                        Latitude = (float)root.GetProperty("latitude").GetDouble(),
                        Longitude = (float)root.GetProperty("longitude").GetDouble(),
                        Region = root.GetProperty("region").GetString()!,
                        RegionCode = root.GetProperty("region_code").GetString()!,
                        TimeZone = root.GetProperty("timezone").GetString()!,
                        ZipCode = root.GetProperty("postal").GetString()!,
                        Provider = Host,
                    };

                    var utc = root.GetProperty("utc_offset").GetString()!;

                    var f = utc[0] switch
                    {
                        '+' => +1,
                        '-' => -1,
                        _ => +1
                    };

                    var hours = int.Parse(utc.Substring(1, 2));
                    var minutes = int.Parse(utc.Substring(4, 2));

                    location.TimeZoneOffset = f * (hours * 60 + minutes);

                    return new(location);
                }

                return new() { Error = $"HTTP {response.StatusCode} - {response.ReasonPhrase} from {Host}", };
            }
            catch
            {

            }

            return new() { Error = Text.Original("Unable to retrieve IP location") };
        }
    }
}
