using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core;
using Origami.Core.Models;

namespace Origami.Core.Data;

public class SettingsRepository :
    RepositoryOuterLayer<OrigamiSettings>,
    ISettingsRepository
{
    private readonly ISettingRepository _settingRepository;
    private readonly IValidator<OrigamiSettings> _validator;

    /// <summary>
    /// Default constructor with DI
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="distributedCache"></param>
    public SettingsRepository(
        IDbContextFactory<OrigamiDbContext> dbContextFactory,
        IMyMemoryCache memoryCache,
        ISettingRepository settingRepository,
        IValidator<OrigamiSettings> validator,
        Text text,
        IWebRootPath wwwRoot)
        : base(text, dbContextFactory, memoryCache, wwwRoot)
    {
        _settingRepository = settingRepository;
        _validator = validator;
    }

    public override string CreatePermission => nameof(OrigamiRole.ViewSettings);
    public override string ReadPermission => nameof(OrigamiRole.ViewSettings);
    public override string UpdatePermission => nameof(OrigamiRole.ViewSettings);

    public override Result<OrigamiSettings> Create(DataOperationContext<OrigamiSettings> ctx)
    {
        var result = new Result<OrigamiSettings>(ctx.Entity);

        var settings = ctx.Entity.GetSettings();
        var contexts = settings.GetContexts(ctx);

        foreach (var context in contexts)
        {
            _settingRepository.SmartSave(context, false).Push(result);
        }

        return result;
    }

    public override void CreateCache(OrigamiSettings entity)
    {
        entity.GetSettings().Each(_settingRepository.CreateCache);
        base.CreateCache(entity);
    }

    public override Result<OrigamiSettings> CreateValidation(DataOperationContext<OrigamiSettings> ctx)
    {
        return new(ctx.Entity, _validator);
    }

    public OrigamiSettings GetSettings()
    {
        var key = $"entity-{typeof(OrigamiSettings).FullName}";

        if (MemoryCache.TryGetValue(key, out OrigamiSettings? settings) == true && settings != null)
        {
            return settings;
        }

        return MemoryCache.Set(key, ExtractSettings(), new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });
    }

    public override Result<OrigamiSettings> Update(DataOperationContext<OrigamiSettings> ctx)
    {
        using var db = DbContextFactory.CreateDbContext();
        var hub = new Result<OrigamiSettings>(ctx.Entity);
        try
        {
            var uiSettings = ctx.Entity.GetSettings();
            var dbSettings = db.Set<OrigamiSetting>().AsNoTracking().ToList();
            var merge = dbSettings.GetMerge(uiSettings);
            this._settingRepository.Merge(ctx, merge).Push(hub);
            return hub;
        }
        finally
        {
            hub.OnSuccess(() => hub.Info = Text.Original("Refresh your browser"));
            hub.OnSuccess(() => hub.Info = Text.Original("If you changed social network settings, you have to restart front-end to take effect"));
            hub.OnSuccess(() => hub.Info = Text.Original("If you changed open telemetry settings, you have to restart both admin and front-end to take effect"));
        }
    }

    public override void UpdateCache(OrigamiSettings entity)
    {
        var key = KeyForCaching;

        MemoryCache.Remove(key);
        MemoryCache.Set(key, entity);

        var uiSettings = entity.GetSettings();
        var dbSettings = _settingRepository.ReadFromCache();
        var merge = dbSettings.GetMerge(uiSettings);
        _settingRepository.MergeCache(merge);
    }

    public override Result<OrigamiSettings> UpdateValidation(DataOperationContext<OrigamiSettings> ctx)
    {
        return new(ctx.Entity, _validator);
    }

    protected OrigamiSettings ExtractSettings()
    {
        using var db = DbContextFactory.CreateDbContext();
        var dbSettings = db.Set<OrigamiSetting>().AsNoTracking().ToList();
        var settings = new OrigamiSettings() { Id = new Guid("9B44A384-4A6C-4095-A797-0C175DC8A4F6") };

        //iterates through all blogsetting's properties
        foreach (var property in settings.GetType().GetProperties())
        {
            if (property.CanRead == false) continue;
            if (property.CanWrite == false) continue;
            if (property.Name.Like(nameof(OrigamiSettings.Id)) == true) continue;
            if (property.Name.Like(nameof(OrigamiSettings.OpenTelemetry)) == true) continue;
            if (property.Name.Like(nameof(OrigamiSettings.SocialNetwork)) == true) continue;
            if (property.Name.Like(nameof(OrigamiSettings.Seq)) == true) continue;

            var name = property.Name.ToLower();
            var setting = dbSettings.FirstOrDefault(x => x.Name == name);
            if (setting != null)
            {
                var value1 = setting.Value;

                if (property.PropertyType.IsEnum)
                {
                    Enum.TryParse(property.PropertyType, value1, out var value2);
                    property.SetValue(settings, value2);
                }
                else
                {
                    var value2 = Convert.ChangeType(value1, property.PropertyType);
                    property.SetValue(settings, value2);
                }
            }
        }

        if (settings.OpenTelemetry != null)
        {
            var prefix = "opentelemetry";
            var otelEnabled = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-enabled"));
            var otelEndpoint = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-endpoint"));
            settings.OpenTelemetry.Enabled = bool.Parse(otelEnabled?.Value ?? "false");
            settings.OpenTelemetry.Endpoint = otelEndpoint?.Value ?? string.Empty;
        }

        if (settings.SocialNetwork.Facebook != null)
        {
            var prefix = "socialnetwork-facebook";
            var facebookEnabled = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-enabled"));
            var facebookAppId = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-appid"));
            var facebookAppSecret = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-appsecret"));
            settings.SocialNetwork.Facebook.Enabled = bool.Parse(facebookEnabled?.Value ?? "false");
            settings.SocialNetwork.Facebook.AppId = facebookAppId?.Value ?? string.Empty;
            settings.SocialNetwork.Facebook.AppSecret = facebookAppSecret?.Value ?? string.Empty;
        }

        if (settings.SocialNetwork.GitHub != null)
        {
            var prefix = "socialnetwork-github";
            var githubEnabled = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-enabled"));
            var githubAppName = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-appname"));
            var githubClientId = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-clientid"));
            var githubClientSecret = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-clientsecret"));
            settings.SocialNetwork.GitHub.Enabled = bool.Parse(githubEnabled?.Value ?? "false");
            settings.SocialNetwork.GitHub.AppName = githubAppName?.Value ?? string.Empty;
            settings.SocialNetwork.GitHub.ClientId = githubClientId?.Value ?? string.Empty;
            settings.SocialNetwork.GitHub.ClientSecret = githubClientSecret?.Value ?? string.Empty;
        }

        if (settings.SocialNetwork.Google != null)
        {
            var prefix = "socialnetwork-google";
            var googleEnabled = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-enabled"));
            var googleApiKey = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-apikey"));
            var googleClientId = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-clientid"));
            var googleClientSecret = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-clientsecret"));
            settings.SocialNetwork.Google.Enabled = bool.Parse(googleEnabled?.Value ?? "false");
            settings.SocialNetwork.Google.ApiKey = googleApiKey?.Value ?? string.Empty;
            settings.SocialNetwork.Google.ClientId = googleClientId?.Value ?? string.Empty;
            settings.SocialNetwork.Google.ClientSecret = googleClientSecret?.Value ?? string.Empty;
        }

        if (settings.SocialNetwork.Microsoft != null)
        {
            var prefix = "socialnetwork-microsoft";
            var microsoftEnabled = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-enabled"));
            var microsoftClientId = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-clientid"));
            var microsoftClientSecret = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-clientsecret"));
            var microsoftTenantId = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-tenantid"));
            settings.SocialNetwork.Microsoft.Enabled = bool.Parse(microsoftEnabled?.Value ?? "false");
            settings.SocialNetwork.Microsoft.ClientId = microsoftClientId?.Value ?? string.Empty;
            settings.SocialNetwork.Microsoft.ClientSecret = microsoftClientSecret?.Value ?? string.Empty;
            settings.SocialNetwork.Microsoft.TenantId = microsoftTenantId?.Value ?? string.Empty;
        }

        if (settings.Seq != null)
        {
            var prefix = "seq";
            var seqEnabled = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-enabled"));
            var seqEndpoint = dbSettings.FirstOrDefault(x => x.Name.Like($"{prefix}-endpoint"));
            settings.Seq.Enabled = bool.Parse(seqEnabled?.Value ?? "false");
            settings.Seq.Endpoint = seqEndpoint?.Value ?? string.Empty;
        }

        return settings;
    }
}
