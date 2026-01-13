using Microsoft.Extensions.Configuration;
using Origami.Core.Data;

namespace Origami.UI;

public class DbConfigurationSource : IConfigurationSource
{
    private readonly ISuperRepository _superRepository;

    public DbConfigurationSource(ISuperRepository superRepository)
    {
        _superRepository = superRepository;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DbConfigurationProvider(_superRepository);
    }
}
