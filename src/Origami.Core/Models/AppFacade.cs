
using Microsoft.Extensions.Logging;
using NanoidDotNet;

namespace Origami.Core.Models
{
    public class AppFacade : IAppFacade
    {
        public AppFacade(bool admin, string environmentName, ILogger<AppFacade> logger) : base()
        {
            this.Admin = admin;
            this.EnvironmentName = environmentName;
            this.OnlineUsers = [];

            var oneTimeMasterPassword = Nanoid.Generate(size: 10);
            logger.LogWarning("One-time master password: {OneTimeMasterPassword}", oneTimeMasterPassword);
            this.OneTimeMasterPasswordInSHA256 = oneTimeMasterPassword.SHA256Hash();
        }

        public event EventHandler<object>? RefreshingTheUI;

        public bool? Admin { get; }

        public string EnvironmentName { get; }

        public string OneTimeMasterPasswordInSHA256 { get; set; }

        public IList<string> OnlineUsers { get; }

        public void RefreshUI(string key)
        {
            this.RefreshingTheUI?.Invoke(key, EventArgs.Empty);
        }

        public void RefreshUI(string connectionId, string key)
        {
            this.RefreshingTheUI?.Invoke(new[] { connectionId, key }, EventArgs.Empty);
        }

        public void RefreshUI(string key, object data)
        {
            this.RefreshingTheUI?.Invoke(key, data);
        }

        public void RefreshUI(string connectionId, string key, object data)
        {
            this.RefreshingTheUI?.Invoke(new[] { connectionId, key }, data);
        }
    }
}
