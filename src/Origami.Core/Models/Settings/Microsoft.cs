namespace Origami.Core.Models.Settings
{
    public class Microsoft : IEnabled, IClientId, IClientSecret, ICallbackPath
    {
        public string CallbackPath { get; set; } = "/signin-oidc";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public string TenantId { get; set; } = string.Empty;
    }
}
