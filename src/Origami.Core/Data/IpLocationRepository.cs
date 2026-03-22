using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class IpLocationRepository : IIpLocationRepository
    {
        protected readonly IMemoryCache _memoryCache;
        protected readonly IIpLocationRepository _ipApiCo;
        protected readonly IIpLocationRepository _ipApiCom;
        protected readonly IIpLocationRepository _ipWhoIs;
        protected IEnumerable<IIpLocationRepository> _locationProviders;
        protected Text _text;

        public IpLocationRepository(
            IMemoryCache memoryCache,
            Text text,
            [FromKeyedServices(IpApiComRepository.Host)] IIpLocationRepository ipApiCom,
            [FromKeyedServices(IpApiCoRepository.Host)] IIpLocationRepository ipApiCo,
            [FromKeyedServices(IpWhoIsRepository.Host)] IIpLocationRepository ipWhoIs) : base()
        {
            this._memoryCache = memoryCache;
            this._ipApiCo = ipApiCo;
            this._ipApiCom = ipApiCom;
            this._ipWhoIs = ipWhoIs;
            this._locationProviders = [ipWhoIs, ipApiCom, ipApiCo];
            this._text = text;
        }

        public async Task<Result<Location>> GetLocationByIpAsync(string ip)
        {
            var hub = new Result<Location>();
            foreach (var provider in this._locationProviders)
            {
                var result = await provider.GetLocationByIpAsync(ip);
                if (result.Ok) return result;
                result.Push(hub);
            }
            hub.Error = _text.Original("All location providers failed");
            return hub;
        }
    }
}
