namespace Origami.Core.Models.Settings
{
    public class Facebook : IEnabled, ICallbackPath
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public string CallbackPath { get; set; } = "/signin-facebook";
        public bool Enabled { get; set; } = true;
    }
}
