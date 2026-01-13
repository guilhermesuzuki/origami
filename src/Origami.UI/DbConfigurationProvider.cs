using Microsoft.Extensions.Configuration;
using Origami.Core.Data;

namespace Origami.UI;

public class DbConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly TimeSpan _checkInterval = new TimeSpan(0, 1, 30);
    private readonly object _lock = new();
    private readonly ISuperRepository _superRepository;
    private readonly Timer _timer;

    public DbConfigurationProvider(ISuperRepository superRepository)
    {
        _superRepository = superRepository;
        _timer = new Timer(CheckForChanges!, null, _checkInterval, _checkInterval);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public override void Load()
    {
        var settings = this._superRepository.Settings.ReadFromDatabase().First();

        this.Data["Site:Name"] = settings.Name.ToLower();
        this.Data["OpenTelemetry:Enabled"] = settings.OpenTelemetry.Enabled.ToString();
        this.Data["OpenTelemetry:Endpoint"] = settings.OpenTelemetry.Endpoint.ToString();

        IList<(string key, string value)> socialNetwork = new List<(string key, string value)>();

        socialNetwork.Add(("SocialNetwork:Facebook:Enabled", settings.SocialNetwork.Facebook.Enabled.ToString()));
        socialNetwork.Add(("SocialNetwork:Facebook:AppId", settings.SocialNetwork.Facebook.AppId));
        socialNetwork.Add(("SocialNetwork:Facebook:AppSecret", settings.SocialNetwork.Facebook.AppSecret));
        socialNetwork.Add(("SocialNetwork:Facebook:CallbackPath", settings.SocialNetwork.Facebook.CallbackPath));

        socialNetwork.Add(("SocialNetwork:GitHub:Enabled", settings.SocialNetwork.GitHub.Enabled.ToString()));
        socialNetwork.Add(("SocialNetwork:GitHub:AppName", settings.SocialNetwork.GitHub.AppName));
        socialNetwork.Add(("SocialNetwork:GitHub:CallbackPath", settings.SocialNetwork.GitHub.CallbackPath));
        socialNetwork.Add(("SocialNetwork:GitHub:ClientId", settings.SocialNetwork.GitHub.ClientId));
        socialNetwork.Add(("SocialNetwork:GitHub:ClientSecret", settings.SocialNetwork.GitHub.ClientSecret));

        socialNetwork.Add(("SocialNetwork:Google:Enabled", settings.SocialNetwork.Google.Enabled.ToString()));
        socialNetwork.Add(("SocialNetwork:Google:ApiKey", settings.SocialNetwork.Google.ApiKey));
        socialNetwork.Add(("SocialNetwork:Google:CallbackPath", settings.SocialNetwork.Google.CallbackPath));
        socialNetwork.Add(("SocialNetwork:Google:ClientId", settings.SocialNetwork.Google.ClientId));
        socialNetwork.Add(("SocialNetwork:Google:ClientSecret", settings.SocialNetwork.Google.ClientSecret));

        socialNetwork.Add(("SocialNetwork:Microsoft:Enabled", settings.SocialNetwork.Microsoft.Enabled.ToString()));
        socialNetwork.Add(("SocialNetwork:Microsoft:CallbackPath", settings.SocialNetwork.Microsoft.CallbackPath));
        socialNetwork.Add(("SocialNetwork:Microsoft:ClientId", settings.SocialNetwork.Microsoft.ClientId));
        socialNetwork.Add(("SocialNetwork:Microsoft:ClientSecret", settings.SocialNetwork.Microsoft.ClientSecret));
        socialNetwork.Add(("SocialNetwork:Microsoft:TenantId", settings.SocialNetwork.Microsoft.TenantId));

        foreach (var (key, value) in socialNetwork)
        {
            this.Data[key] = value;
        }
    }

    private void CheckForChanges(object? _)
    {
        lock (_lock)
        {
            Load();
            OnReload();
        }
    }
}
