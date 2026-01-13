namespace Origami.Core.Models.Settings
{
    public class Google : IEnabled, IClientId, IClientSecret, ICallbackPath
    {
        public string ApiKey { get; set; } = string.Empty;
        public string CallbackPath { get; set; } = "/signin-google";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }
}
