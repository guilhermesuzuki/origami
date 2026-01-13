using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IIpLocationRepository
    {
        Task<Result<Location>> GetLocationByIpAsync(string ip);
    }
}
