using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface IRightRepository : IRepository<OrigamiRight>
    {
        Result KeepUpToDate();
    }
}
