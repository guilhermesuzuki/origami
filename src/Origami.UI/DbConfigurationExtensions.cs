using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Origami.Core.Data;

namespace Origami.UI;

public static class DbConfigurationExtensions
{
    public static IConfigurationBuilder AddDatabase(this IConfigurationBuilder builder, IServiceProvider serviceProvider)
    {
        var super = serviceProvider.GetRequiredService<ISuperRepository>();
        return builder.Add(new DbConfigurationSource(super));
    }
}
