namespace Origami.Core.Models.Settings
{
    public class GitHub : IEnabled, IClientId, IClientSecret, ICallbackPath
    {
        public string AppName { get; set; } = string.Empty;
        public string CallbackPath { get; set; } = "/signin-github";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }
}
