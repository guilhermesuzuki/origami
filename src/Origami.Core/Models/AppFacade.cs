

namespace Origami.Core.Models
{
    public class AppFacade : IAppFacade
    {
        public AppFacade(bool admin) : base()
        {
            this.Admin = admin;
            this.OnlineUsers = [];
        }

        public event EventHandler? RefreshingTheUI;

        public bool? Admin { get; }

        public IList<string> OnlineUsers { get; }

        public void RefreshUI(string key)
        {
            this.RefreshingTheUI?.Invoke(key, EventArgs.Empty);
        }

        public void RefreshUI(string connectionId, string key)
        {
            this.RefreshingTheUI?.Invoke(new[] { connectionId, key }, EventArgs.Empty);
        }
    }
}
