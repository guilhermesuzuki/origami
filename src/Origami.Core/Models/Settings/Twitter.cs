namespace Origami.Core.Models.Settings
{
    public class Twitter : IEnabled
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ApiKeySecret { get; set; } = string.Empty;
        public string BearerToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string AccessTokenSecret { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }
}
